//
// ASCOM ObservingConditions driver for NANO
//
// Description:	 Observing Conditions driver for Arduino Uno and Nano with
//               3 sensors. BMP280 for Pressure I2C, HTU21D for Temperature and Humidity I2C,
//                          TSL237 for SQM with 15º lens.
//                  PoleMaster camera for FWHM using Siril ( batchfile and NINA sequence 2º instance)
// Implements:	ASCOM ObservingConditions interface version: <To be completed by driver developer>
// Author:		(XXX) Lammertus de Vries <your@email.here>
//

using ASCOM;
using ASCOM.Astrometry;
using ASCOM.Astrometry.AstroUtils;
using ASCOM.Astrometry.NOVAS;
using ASCOM.DeviceInterface;
using ASCOM.LocalServer;
using ASCOM.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Runtime.Remoting.Messaging;
using System.Windows.Forms;
using System.Threading;
using System.Drawing;
using System.Security.Cryptography;
using System.Windows.Forms.VisualStyles;
using System.Diagnostics;                          // For starting batch cmd files
using System.IO;
using System.Text;
using ASCOM.Astrometry.NOVASCOM;
using System.Diagnostics.Eventing.Reader;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ASCOM.NANO.ObservingConditions
{
    //
    // TODO Replace the not implemented exceptions with code to implement the function or throw the appropriate ASCOM exception.
    //

    /// <summary>
    /// ASCOM ObservingConditions hardware class for NANO.
    /// </summary>
    [HardwareClass()] // Class attribute flag this as a device hardware class that needs to be disposed by the local server when it exits.
    internal static class ObservingConditionsHardware
    {
        // Constants used for Profile persistence
        internal const string comPortProfileName = "COM Port";
        internal const string comPortDefault = "COM1";
        internal const string traceStateProfileName = "Trace Level";
        internal const string traceStateDefault = "True";
        internal const string FWHMpathProfileName = "FWHMpath";
        internal static string FWHMpathDefault = "C:\\";
        internal const string BatchfilepathProfileName = "Batchfile";
        internal static string BatchfilepathDefault = "C:\\";
        internal const string BatchfileProfileName = "NameBatchfile";
        internal static string BatchfileDefault = "psf3.bat";
        internal const string SensorLogfileProfileName = "Sensorlog";
        internal static string SensorLogfileDefault = "Sensorlog.txt";
        internal const string LogFileStateProfileName = "SensorLogState";
        internal const string LogFileStateDefault = "False";
        public static string SensorLogEnable = "False";
        public static string logfileString = "";
        public const string UpdateSampleTimeProfileName = "SampleTime";
        public static string UpdateSampleTimeDefault = "30";
        public const string CorrFactorScopeProfileName = "FWHM-scope-factor";
        public static string CorrFactorScopeDefault = "42";   // More or less for Polemaster with 25mm Focallength
        public static int HeadertextWritten = 0;    // First time log the header text has to be written

        private static string DriverProgId = ""; // ASCOM DeviceID (COM ProgID) for this driver, the value is set by the driver's class initialiser.
        private static string DriverDescription = ""; // The value is set by the driver's class initialiser.
        internal static string comPort; // COM port name (if required)
        private static bool connectedState; // Local server's connected state
        private static bool runOnce = false; // Flag to enable "one-off" activities only to run once.
        internal static Util utilities; // ASCOM Utilities object for use as required
        internal static AstroUtils astroUtilities; // ASCOM AstroUtilities object for use as required
        internal static TraceLogger tl; // Local server's trace logger object for diagnostic log with information that you specify

        /// <summary>
        /// Initializes a new instance of the device Hardware class.
        /// </summary>
        static ObservingConditionsHardware()
        {
            try
            {
                // Create the hardware trace logger in the static initialiser.
                // All other initialisation should go in the InitialiseHardware method.
                tl = new TraceLogger("", "NANO.Hardware");

                // DriverProgId has to be set here because it used by ReadProfile to get the TraceState flag.
                DriverProgId = ObservingConditions.DriverProgId; // Get this device's ProgID so that it can be used to read the Profile configuration values

                // ReadProfile has to go here before anything is written to the log because it loads the TraceLogger enable / disable state.
                ReadProfile(); // Read device configuration from the ASCOM Profile store, including the trace state

                LogMessage("ObservingConditionsHardware", $"Static initialiser completed.");
            }
            catch (Exception ex)
            {
                try { LogMessage("ObservingConditionsHardware", $"Initialisation exception: {ex}"); } catch { }
                MessageBox.Show($"{ex.Message}", "Exception creating ASCOM.NANO.ObservingConditions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        /// <summary>
        /// Place device initialisation code here that delivers the selected ASCOM <see cref="Devices."/>
        /// </summary>
        /// <remarks>Called every time a new instance of the driver is created.</remarks>
        internal static void InitialiseHardware()
        {
            // This method will be called every time a new ASCOM client loads your driver
            LogMessage("InitialiseHardware", $"Start.");

            // Make sure that "one off" activities are only undertaken once
            if (runOnce == false)
            {
                LogMessage("InitialiseHardware", $"Starting one-off initialisation.");

                DriverDescription = ObservingConditions.DriverDescription; // Get this device's Chooser description

                LogMessage("InitialiseHardware", $"ProgID: {DriverProgId}, Description: {DriverDescription}");

                connectedState = false; // Initialise connected to false
                utilities = new Util(); //Initialise ASCOM Utilities object
                astroUtilities = new AstroUtils(); // Initialise ASCOM Astronomy Utilities object

                LogMessage("InitialiseHardware", "Completed basic initialisation");

                // Add your own "one off" device initialisation here e.g. validating existence of hardware and setting up communications

 
                LogMessage("InitialiseHardware", $"One-off initialisation complete.");
                runOnce = true; // Set the flag to ensure that this code is not run again
            }
        }

        // PUBLIC COM INTERFACE IObservingConditions IMPLEMENTATION

        #region Common properties and methods.

        /// <summary>
        /// Displays the Setup Dialogue form.
        /// If the user clicks the OK button to dismiss the form, then
        /// the new settings are saved, otherwise the old values are reloaded.
        /// THIS IS THE ONLY PLACE WHERE SHOWING USER INTERFACE IS ALLOWED!
        /// </summary>
        public static void SetupDialog()
        {
            // Don't permit the setup dialogue if already connected
            if (IsConnected)
                MessageBox.Show("Already connected, just press OK");

            using (SetupDialogForm F = new SetupDialogForm(tl))
            {
                var result = F.ShowDialog();
                if (result == DialogResult.OK)
                {
                    WriteProfile(); // Persist device configuration values to the ASCOM Profile store
                }
            }
        }

        /// <summary>Returns the list of custom action names supported by this driver.</summary>
        /// <value>An ArrayList of strings (SafeArray collection) containing the names of supported actions.</value>
        public static ArrayList SupportedActions
        {
            get
            {
                // LogMessage("SupportedActions Get", "Returning empty ArrayList");
                ArrayList supportedActions = new ArrayList();
                supportedActions.Add("GetTemp");
                supportedActions.Add("GetDew");
                supportedActions.Add("GetHum");
                supportedActions.Add("GetPressure");
                supportedActions.Add("GetSkyQuality");  // SQM value
                supportedActions.Add("GetFWHM");        // FWHM value
                return supportedActions;
            }
        }

        /// <summary>Invokes the specified device-specific custom action.</summary>
        /// <param name="ActionName">A well known name agreed by interested parties that represents the action to be carried out.</param>
        /// <param name="ActionParameters">List of required parameters or an <see cref="String.Empty">Empty String</see> if none are required.</param>
        /// <returns>A string response. The meaning of returned strings is set by the driver author.
        /// <para>Suppose filter wheels start to appear with automatic wheel changers; new actions could be <c>QueryWheels</c> and <c>SelectWheel</c>. The former returning a formatted list
        /// of wheel names and the second taking a wheel name and making the change, returning appropriate values to indicate success or failure.</para>
        /// </returns>
        /// 
                public static string Action(string actionName, string actionParameters)
        {
            LogMessage("Action", $"Action {actionName}, parameters {actionParameters} is not implemented");
            throw new ActionNotImplementedException("Action " + actionName + " is not implemented by this driver");
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and does not wait for a response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="Command">The literal command string to be transmitted.</param>
        /// <param name="Raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        public static void CommandBlind(string command, bool raw)
        {
            CheckConnected("CommandBlind");
            // TODO The optional CommandBlind method should either be implemented OR throw a MethodNotImplementedException
            // If implemented, CommandBlind must send the supplied command to the mount and return immediately without waiting for a response

            throw new MethodNotImplementedException($"CommandBlind - Command:{command}, Raw: {raw}.");
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and waits for a boolean response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="Command">The literal command string to be transmitted.</param>
        /// <param name="Raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        /// <returns>
        /// Returns the interpreted boolean response received from the device.
        /// </returns>
        public static bool CommandBool(string command, bool raw)
        {
            CheckConnected("CommandBool");
            // TODO The optional CommandBool method should either be implemented OR throw a MethodNotImplementedException
            // If implemented, CommandBool must send the supplied command to the mount, wait for a response and parse this to return a True or False value

            throw new MethodNotImplementedException($"CommandBool - Command:{command}, Raw: {raw}.");
        }

        /// <summary>
        /// Transmits an arbitrary string to the device and waits for a string response.
        /// Optionally, protocol framing characters may be added to the string before transmission.
        /// </summary>
        /// <param name="Command">The literal command string to be transmitted.</param>
        /// <param name="Raw">
        /// if set to <c>true</c> the string is transmitted 'as-is'.
        /// If set to <c>false</c> then protocol framing characters may be added prior to transmission.
        /// </param>
        /// <returns>
        /// Returns the string response received from the device.
        /// </returns>
        public static string CommandString(string command, bool raw)
        {
            CheckConnected("CommandString");
            // TODO The optional CommandString method should either be implemented OR throw a MethodNotImplementedException
            // If implemented, CommandString must send the supplied command to the mount and wait for a response before returning this to the client

            throw new MethodNotImplementedException($"CommandString - Command:{command}, Raw: {raw}.");
        }

        /// <summary>
        /// Deterministically release both managed and unmanaged resources that are used by this class.
        /// </summary>
        /// <remarks>
        /// TODO: Release any managed or unmanaged resources that are used in this class.
        /// 
        /// Do not call this method from the Dispose method in your driver class.
        ///
        /// This is because this hardware class is decorated with the <see cref="HardwareClassAttribute"/> attribute and this Dispose() method will be called 
        /// automatically by the  local server executable when it is irretrievably shutting down. This gives you the opportunity to release managed and unmanaged 
        /// resources in a timely fashion and avoid any time delay between local server close down and garbage collection by the .NET runtime.
        ///
        /// For the same reason, do not call the SharedResources.Dispose() method from this method. Any resources used in the static shared resources class
        /// itself should be released in the SharedResources.Dispose() method as usual. The SharedResources.Dispose() method will be called automatically 
        /// by the local server just before it shuts down.
        /// 
        /// </remarks>
        public static void Dispose()
        {
            try { LogMessage("Dispose", $"Disposing of assets and closing down."); } catch { }

            try
            {
                // Clean up the trace logger and utility objects
                tl.Enabled = false;
                tl.Dispose();
                tl = null;
            }
            catch { }

            try
            {
                utilities.Dispose();
                utilities = null;
            }
            catch { }

            try
            {
                astroUtilities.Dispose();
                astroUtilities = null;
            }
            catch { }
        }

        /// <summary>
        /// Set True to connect to the device hardware. Set False to disconnect from the device hardware.
        /// You can also read the property to check whether it is connected. This reports the current hardware state.
        /// </summary>
        /// <value><c>true</c> if connected to the hardware; otherwise, <c>false</c>.</value>
        public static bool Connected
        {
            get
            {
                LogMessage("Connected", $"Get {IsConnected}");
                return IsConnected;
            }
            set
            {
                LogMessage("Connected", $"Set {value}");
                if (value == IsConnected)
                    return;

                if (value)
                {
                    LogMessage("Connected Set", $"Connecting to port {comPort}");

                    // TODO insert connect to the device code here
 
                    LocalServer.SharedResources.Connected = true;
                    return;
                }
                else
                {
                    LogMessage("Connected Set", $"Disconnecting from port {comPort}");

                    // TODO insert disconnect from the device code here

                    LocalServer.SharedResources.Connected = false;
                    //connectedState = false;
                }
            }
        }

        /// <summary>
        /// Returns a description of the device, such as manufacturer and model number. Any ASCII characters may be used.
        /// </summary>
        /// <value>The description.</value>
        public static string Description
        {
            // TODO customise this device description if required
            get
            {
                LogMessage("Description Get", DriverDescription);
                return DriverDescription;
            }
        }

        /// <summary>
        /// Descriptive and version information about this ASCOM driver.
        /// </summary>
        public static string DriverInfo
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                // TODO customise this driver description if required
                string driverInfo = $"ArduinoNANO ObsCon. Version: {version.Major}.{version.Minor}";
                LogMessage("DriverInfo Get", driverInfo);
                return driverInfo;
            }
        }

        /// <summary>
        /// A string containing only the major and minor version of the driver formatted as 'm.n'.
        /// </summary>
        public static string DriverVersion
        {
            get
            {
                Version version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                string driverVersion = $"{version.Major}.{version.Minor}";
                LogMessage("DriverVersion Get", driverVersion);
                return driverVersion;
            }
        }

        /// <summary>
        /// The interface version number that this device supports.
        /// </summary>
        public static short InterfaceVersion
        {
            // set by the driver wizard
            get
            {
                LogMessage("InterfaceVersion Get", "1");
                return Convert.ToInt16("1");
            }
        }

        /// <summary>
        /// The short name of the driver, for display purposes
        /// </summary>
        public static string Name
        {
            // TODO customise this device name as required
            get
            {
                string name = "ArduinoNANO ObsCon";
                LogMessage("Name Get", name);
                return name;
            }
        }

        #endregion

        #region IObservingConditions Implementation

        // Time and wind speed values
        private static Dictionary<DateTime, double> winds = new Dictionary<DateTime, double>();

        /// <summary>
        /// Gets and sets the time period over which observations will be averaged
        /// </summary>
        internal static double AveragePeriod
        {
            get
            {
                LogMessage("AveragePeriod", "get - 0");
                return 10;
            }
            set
            {
                LogMessage("AveragePeriod Set", value.ToString());
                if (value < 0)
                    throw new InvalidValueException("AveragePeriod", value.ToString(), "0 only");
                {
                    return;
                }
            }
        }

        /// <summary>
        /// Amount of sky obscured by cloud
        /// </summary>
        internal static double CloudCover
        {
            get
            {
                // LogMessage("CloudCover", "get - not implemented");
                // throw new PropertyNotImplementedException("CloudCover", false);
                if ( SensorLogEnable == "True")
                {
                    logfileString = DateTime.Now + "; 0";
                }
                return 0;
            }
        }

        /// <summary>
        /// Atmospheric dew point at the observatory in deg C
        /// </summary>
        /// 

        internal static double DewPoint
        {
            get
            {
                if (!Connected)
                {
                    string ReadDewpoint, dewPoint = string.Empty;
                    LocalServer.SharedResources.Connected = true;
                    ReadDewpoint =  LocalServer.SharedResources.SendMessage("1GetDew#");
                    dewPoint = ReadDewpoint.Remove(ReadDewpoint.Length - 1, 1);
                    if ( SensorLogEnable == "True") {       // Write value to logfile-string
                             logfileString = logfileString + ";" + dewPoint;
                    }
                    Thread.Sleep(15000);
                    return Double.Parse(dewPoint);
                }
                else
                {
                     LogMessage("DewPoint", "get - not implemented");
                     throw new PropertyNotImplementedException("DewPoint", false);
                }
            }
            set
            {
                LocalServer.SharedResources.Connected = false;
            }
        }

        /// <summary>
        /// Atmospheric relative humidity at the observatory in percent
        /// </summary>
        internal static double Humidity
        {
            get
            {               
                if (!Connected)
                    {
                //        Thread.Sleep(500); 
                     string ReadHumidity, humidity = string.Empty;
                     LocalServer.SharedResources.Connected = true;
                     ReadHumidity = LocalServer.SharedResources.SendMessage("7HtuHum#");
                     humidity = ReadHumidity.Remove(ReadHumidity.Length - 1, 1);
                     if (SensorLogEnable == "True")
                        {       // Write value to logfile-string
                            logfileString = logfileString + ";" + humidity;
                        }
                     return Double.Parse(humidity);
                     }
                else
                    {
                        LogMessage("Humidity", "get - not implemented");
                        throw new PropertyNotImplementedException("Humidity", false);
                    }               
                }
            set
            {
                LocalServer.SharedResources.Connected = false;
            }
        }

        /// <summary>
        /// Atmospheric pressure at the observatory in hectoPascals (mB)
        /// </summary>
        internal static double Pressure
        {
            get
            {
                //  LogMessage("Pressure", "get - not implemented");
                //  throw new PropertyNotImplementedException("Pressure", false);
                if (!Connected)
                {
                //   Thread.Sleep(500);
                    string ReadPressure, pressure = string.Empty;
                    LocalServer.SharedResources.Connected = true;
                    ReadPressure = LocalServer.SharedResources.SendMessage("2GetPressure#");
                    pressure = ReadPressure.Remove(ReadPressure.Length - 1, 1);
                    if (SensorLogEnable == "True")
                    {       // Write value to logfile-string
                        logfileString = logfileString + ";" + pressure;
                    }
                    return Double.Parse(pressure);
                }
                else
                {
                    LogMessage("Pressure", "get - not implemented");
                    throw new PropertyNotImplementedException("Pressure", false);
                    return 0;
                }               
            }
        }

        internal static double Altitude
        {
            get
            {
                //  LogMessage("Altitude", "get - not implemented");
                //  throw new PropertyNotImplementedException("Altitude", false);
                if (!Connected)
                {
                //    Thread.Sleep(500); 
                    string ReadAltitude, altitude = string.Empty;
                    LocalServer.SharedResources.Connected = true;
                    ReadAltitude = LocalServer.SharedResources.SendMessage("3GetAltitude#");
                    altitude = ReadAltitude.Remove(ReadAltitude.Length - 1, 1);
                    if (SensorLogEnable == "True")
                    {       // Write value to logfile-string
                    //    logfileString = logfileString + ";" + altitude;
                    }
                    return Double.Parse(altitude);
                }
                else
                {
                    LogMessage("Altitude", "get - not implemented");
                    throw new PropertyNotImplementedException("Altitude", false);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Rain rate at the observatory
        /// </summary>
        internal static double RainRate
        {
            get
            {
                // LogMessage("RainRate", "get - not implemented");
                // throw new PropertyNotImplementedException("RainRate", false);
                if (SensorLogEnable == "True")
                {       // Write value to logfile-string
                    logfileString = logfileString + ";" + "0";
                }
                return 0;
            }
        }

        /// <summary>
        /// Forces the driver to immediately query its attached hardware to refresh sensor
        /// values
        /// </summary>
        internal static void Refresh()
        {
            throw new MethodNotImplementedException();
        }

        /// <summary>
        /// Provides a description of the sensor providing the requested property
        /// </summary>
        /// <param name="propertyName">Name of the property whose sensor description is required</param>
        /// <returns>The sensor description string</returns>
        internal static string SensorDescription(string propertyName)
        {
            switch (propertyName.Trim().ToLowerInvariant())
            {
                case "averageperiod":
                    return "Average period in hours, immediate values are only available";
                case "cloudcover":
                    return "Not connected";
                    break;
                case "dewpoint":
                    return "HTU21D sensor";
                    break;
                case "humidity":
                    return "HTU21D sensor";
                    break;
                case "pressure":
                    return "BMP280 sensor";
                    break;
                case "rainrate":
                    return "Not connected";
                    break;
                case "skybrightness":
                    return "Not connected";
                    break;
                case "skyquality":
                    return "TSL237 sensor";
                    break;
                case "skytemperature":
                    return "Not connected";
                    break;
                case "starfwhm":
                    return "QHYCCD Polemaster";
                    break;
                case "temperature":
                    return "HTU21D sensor";
                    break;
                case "altitude":
                    return "BMP280 sensor";
                    break;
                case "winddirection":
                    return "Not connected";
                    break;
                case "windgust":
                    return "Not connected";
                    break;
                case "windspeed":
                    // Throw an exception on the properties that are not implemented
                    //  LogMessage("SensorDescription", $"Property {propertyName} is not implemented");
                    //  throw new MethodNotImplementedException($"SensorDescription - Property {propertyName} is not implemented");
                    return "Not connected";
                    break;
                default:
                    LogMessage("SensorDescription", $"Invalid sensor name: {propertyName}");
                    throw new InvalidValueException($"SensorDescription - Invalid property name: {propertyName}");
            }
        }

        /// <summary>
        /// Sky brightness at the observatory
        /// </summary>
        internal static double SkyBrightness
        {
            get
            {
                //   LogMessage("SkyBrightness", "get - not implemented");
                //   throw new PropertyNotImplementedException("SkyBrightness", false);
                if (SensorLogEnable == "True")
                {       // Write value to logfile-string
                    logfileString = logfileString + ";" + "0";
                }
                return 0;
            }
         }

        /// <summary>
        /// Sky quality at the observatory
        /// </summary>
        internal static double SkyQuality
    {
        get
        {
            //    LogMessage("SkyQuality", "get - not implemented");
            //    throw new PropertyNotImplementedException("SkyQuality", false);
            if (!Connected)
            {
                string ReadSkyQuality, skyquality = string.Empty;
                LocalServer.SharedResources.Connected = true;
                ReadSkyQuality = LocalServer.SharedResources.SendMessage("5GetSkyQuality#");
             //       Thread.Sleep(40000);  // Aprox. <40 seg con SQM 22
                skyquality = ReadSkyQuality.Remove(ReadSkyQuality.Length - 1, 1);
                if (skyquality == "")
                    {
                        skyquality = "20.9";
                    }
                if (SensorLogEnable == "True")
                {       // Write value to logfile-string
                    logfileString = logfileString + ";" + skyquality;
                }
                return Double.Parse(skyquality);
            }
            else
            {
                LogMessage("SkyQuality", "get - not implemented");
                throw new PropertyNotImplementedException("SkyQuality", false);
            }
        }
    }
        /// <summary>
        /// Seeing at the observatory
        /// </summary>
        internal static double StarFWHM
        {
            get
            {
                string OldFile, NewFile, WorkDir;
                String line, fwhmfile;
                long fwhmFileSize;
                int readfwhmindex;
                WorkDir = FWHMpathDefault;
                OldFile = "FWHM.old";
                NewFile = "FWHM.fits";
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                string batch_file_path = BatchfilepathDefault + "\\" + ObservingConditionsHardware.BatchfileDefault;  // For the batchfile to work you need to have Siril installed
                p.StartInfo.FileName = "cmd.exe"; // <-- EXECUTABLE NAME
                p.StartInfo.RedirectStandardOutput = true; // <-- REDIRECT THE STDOUT OF THE SCRIPT FROM SCRIPT TO OS, TO SCRIPT TO C# APPLICATION
                p.StartInfo.Arguments = "/c \"" + batch_file_path + "\""; // <-- COMMAND TO BE RUN BY CMD '/k', and the content of the command "PATH" 
                p.StartInfo.WorkingDirectory = ObservingConditionsHardware.FWHMpathDefault;
                p.StartInfo.CreateNoWindow = true; // <-- CREATE NO WINDOW
                p.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;   // If not there will be 3 windows intermittendly shown
                p.StartInfo.UseShellExecute = false; // <-- USE THE C# APPLICATION AS THE SHELL THROUGH WHICH THE PROCESS IS EXECUTED, NOT THE OS ITSELF

                p.Start(); // <-- START THE APPLICATION
                p.WaitForExit();  // let batchfile terminate

                if (File.Exists(WorkDir + "\\" + OldFile))
                {
                    File.Delete(WorkDir + "\\" + OldFile);
                }
                
                File.Move(WorkDir + "\\" + NewFile, WorkDir + "\\" + OldFile);
                ///  FWHM " = ( FWHM pix * pixelsize (um) * 206.3 ) / Focallength ( mm )
                ///  pixelsize = 3.75 um  and Focallength = 25.29 mm  ( Polemaster )

                fwhmfile = WorkDir + "\\" + "fwhm.txt";  // No funciona!
                fwhmFileSize = new FileInfo(fwhmfile).Length;

                if ( File.Exists(fwhmfile) )    // FileInfo(FilePath).Length
                {
                     double fwhmPixel, fwhmArcsec, Corrfactor;
                    // Example Output: 
                    // log: Found 1754 Gaussian profile stars in image, channel #1 (FWHM 6.158044)
                    Corrfactor = Convert.ToDouble(ObservingConditionsHardware.CorrFactorScopeDefault);
                    FileStream F = new FileStream(WorkDir + "\\" + "fwhm.txt", FileMode.OpenOrCreate);
                    StreamReader sr = new StreamReader(F);
                    line = sr.ReadLine();
                    line.Trim();
                    if( fwhmFileSize == 0 )
                    {
                        logfileString = logfileString + ";" + "-1.0";
                        sr.Close();
                        return -1.0;
                    }

                    if ( SensorLogEnable == "True" )
                    {
                        readfwhmindex = line.IndexOf("FWHM");
                        if ( readfwhmindex < 10 )  // File empty?
                        {
                            logfileString = logfileString + ";" + "-1.0";
                            sr.Close();
                            return -1.0;
                        }
                        else
                        {
                            fwhmPixel = double.Parse(line.Substring(readfwhmindex + 5, 5));
                            fwhmArcsec = ((fwhmPixel * 3.8 * 206.3) / 25.29) / Corrfactor;
                            logfileString = logfileString + ";" + Convert.ToString(fwhmArcsec);
                            sr.Close();
                            return fwhmArcsec;
                        }
                    }
                     readfwhmindex = line.IndexOf("FWHM");
                     fwhmPixel = double.Parse(line.Substring(readfwhmindex + 5, 5));
                     fwhmArcsec = ((fwhmPixel * 3.8 * 206.3) / 25.29) / Corrfactor;
                     return fwhmArcsec;
                }
                else
                {
                    logfileString = logfileString + ";" + "na.";    // No valid response obtained
                    return -1.0;
                }
            }
        }

        /// <summary>
        /// Sky temperature at the observatory in deg C
        /// </summary>
        internal static double SkyTemperature
        {
            get
            {
                Thread.Sleep(500);
                //   LogMessage("SkyTemperature", "get - not implemented");
                //   throw new PropertyNotImplementedException("SkyTemperature", false);
                if (SensorLogEnable == "True")
                {       // Write value to logfile-string
                    logfileString = logfileString + ";" + "-1";
                }
                return -1;
            }
        }

        /// <summary>
        /// Temperature at the observatory in deg C
        /// </summary>
        internal static double Temperature
        {
            get
            {
                if (!Connected)
                {
                    string ReadTemperature, temperature = string.Empty;
                    LocalServer.SharedResources.Connected = true;
                    ReadTemperature = LocalServer.SharedResources.SendMessage("6HtuTemp#");
                    temperature = ReadTemperature.Remove(ReadTemperature.Length - 1, 1);
                    if (SensorLogEnable == "True")
                    {       // Write value to logfile-string
                        logfileString = logfileString + ";" + temperature;
                    }
                    return Double.Parse(temperature);
                 }
                else
                {
                    LogMessage("Temperature", "get - not implemented");
                    throw new PropertyNotImplementedException("Temperature", false);
                }
            }
            set
            {
                LocalServer.SharedResources.Connected = false;
            }
        }

        // Wait defined time to update sensors again


        /// <summary>
        /// Provides the time since the sensor value was last updated
        /// </summary>
        /// <param name="propertyName">Name of the property whose time since last update Is required</param>
        /// <returns>Time in seconds since the last sensor update for this property</returns>
        internal static double TimeSinceLastUpdate(string propertyName)
        {
            // Test for an empty property name, if found, return the time since the most recent update to any sensor
            if (!string.IsNullOrEmpty(propertyName))
            {
                switch (propertyName.Trim().ToLowerInvariant())
                {
                    // Return the time for properties that are implemented, otherwise fall through to the MethodNotImplementedException
                    case "averageperiod":
                        return 10.0;
                        break;
                    case "cloudcover":
                        return -1;
                        break;
                    case "dewpoint":
                        return DateTime.Now.Ticks;
                        break;
                    case "humidity":
                        return DateTime.Now.Ticks;
                        break;
                    case "pressure":
                        return DateTime.Now.Ticks;
                        break;
                    case "rainrate":
                        return -1;
                        break;
                    case "skybrightness":
                        return DateTime.Now.Ticks; 
                        break;
                    case "skyquality":
                        return -1;
                        break;
                    case "skytemperature":
                        return -1;
                        break;
                    case "starfwhm":
                        return DateTime.Now.Ticks;
                        break;
                    case "temperature":
                        return DateTime.Now.Ticks;
                        break;
                    case "altitude":
                        return DateTime.Now.Ticks;
                        break;
                    case "winddirection":
                        return -1;
                        break;
                    case "windgust":
                        return -1;
                        break;
                    case "windspeed":
                        // Throw an exception on the properties that are not implemented
                        // LogMessage("TimeSinceLastUpdate", $"Property {propertyName} is not implemented");
                        // throw new MethodNotImplementedException($"TimeSinceLastUpdate - Property {propertyName} is not implemented");
                        return -1;
                    default:
                        LogMessage("TimeSinceLastUpdate", $"Invalid sensor name: {propertyName}");
                        throw new InvalidValueException($"TimeSinceLastUpdate - Invalid property name: {propertyName}");
                }
            }

            // Return the time since the most recent update to any sensor
            // LogMessage("TimeSinceLastUpdate", $"The time since the most recent sensor update is not implemented");
            // throw new MethodNotImplementedException("TimeSinceLastUpdate(" + propertyName + ")");
            return DateTime.Now.Ticks;
        }

        /// <summary>
        /// Wind direction at the observatory in degrees
        /// </summary>
        internal static double WindDirection
        {
            get
            {
                //   LogMessage("WindDirection", "get - not implemented");
                //   throw new PropertyNotImplementedException("WindDirection", false);
                if (SensorLogEnable == "True")
                {       // Write value to logfile-string
                    logfileString = logfileString + ";" + "0";
                }
                return 0;
            }
        }

        /// <summary>
        /// Peak 3 second wind gust at the observatory over the last 2 minutes in m/s
        /// </summary>
        internal static double WindGust
        {
            get
            {
                //    LogMessage("WindGust", "get - not implemented");
                //    throw new PropertyNotImplementedException("WindGust", false);
                if (SensorLogEnable == "True")
                {       // Write value to logfile-string
                    logfileString = logfileString + ";" + "0";
                }
                return 0;
            }
        }

        /// <summary>
        /// Wind speed at the observatory in m/s
        /// </summary>
        internal static double WindSpeed
        {
            get
            {
                //    LogMessage("WindSpeed", "get - not implemented");
                //    throw new PropertyNotImplementedException("WindSpeed", false);
                if (SensorLogEnable == "True")
                {   
                    string SensorLogfileProfileName = BatchfilepathDefault + "\\" + SensorLogfileDefault;
                    //                    MessageBox.Show(SensorLogfileProfileName);
                    string Headertext = "DateTime;Cloudcover;Dewpoint;Humidity;Pressure;Rainrate;Skybright;SQM;SkyTemp;star FWHM;Temperature;Wind;Wind;Wind";
                    if ( HeadertextWritten == 0)
                    {
                        StreamWriter headerlogfile = File.AppendText(SensorLogfileProfileName);
                        headerlogfile.WriteLine(Headertext);
                        headerlogfile.Close();
                        HeadertextWritten = 1;
                    }

                    logfileString = logfileString + ";" + "0";  //This is the last value queried so now we write out the logfile string
                    StreamWriter logfile = File.AppendText(SensorLogfileProfileName);
                    logfile.AutoFlush = true;
                    logfile.WriteLine(logfileString); // Output all the sensors ; seperated
                    logfile.Close();   // Close logfile
                    logfileString = "";
                }
                 return 0;
            }
        }

        #endregion

        #region Private methods

        #region Calculate the gust strength as the largest wind recorded over the last two minutes


        private static void UpdateGusts(double speed)
        {
            Dictionary<DateTime, double> newWinds = new Dictionary<DateTime, double>();
            var last = DateTime.Now - TimeSpan.FromMinutes(2);
            winds.Add(DateTime.Now, speed);
            var gust = 0.0;
            foreach (var item in winds)
            {
                if (item.Key > last)
                {
                    newWinds.Add(item.Key, item.Value);
                    if (item.Value > gust)
                        gust = item.Value;
                }
            }
            winds = newWinds;
        }

       

        #endregion

        #endregion


        
        #region Private properties and methods sensorLogEnable

       

        #endregion


        // Useful methods that can be used as required to help with driver development

        /// <summary>
        /// Returns true if there is a valid connection to the driver hardware
        /// </summary>
        private static bool IsConnected
        {
            get
            {
                return connectedState;
            }
        }


        /// <summary>
        /// Use this function to throw an exception if we aren't connected to the hardware
        /// </summary>
        /// <param name="message"></param>
        private static void CheckConnected(string message)
        {
            if (!IsConnected)
            {
                throw new NotConnectedException(message);
            }
        }

        /// <summary>
        /// Read the device configuration from the ASCOM Profile store
        /// </summary>
        internal static void ReadProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "ObservingConditions";
                tl.Enabled = Convert.ToBoolean(driverProfile.GetValue(DriverProgId, traceStateProfileName, string.Empty, traceStateDefault));
                comPort = driverProfile.GetValue(DriverProgId, comPortProfileName, string.Empty, comPortDefault);
                FWHMpathDefault = driverProfile.GetValue(DriverProgId, FWHMpathProfileName, string.Empty, FWHMpathDefault);
                BatchfilepathDefault = driverProfile.GetValue(DriverProgId, BatchfilepathProfileName, string.Empty, BatchfilepathDefault);
                BatchfileDefault = driverProfile.GetValue(DriverProgId, BatchfileProfileName, string.Empty, BatchfileDefault);
                SensorLogfileDefault = driverProfile.GetValue(DriverProgId, SensorLogfileProfileName, string.Empty, SensorLogfileDefault);
                SensorLogEnable = driverProfile.GetValue(DriverProgId, LogFileStateProfileName, string.Empty, LogFileStateDefault);
                UpdateSampleTimeDefault = driverProfile.GetValue(DriverProgId, UpdateSampleTimeProfileName, string.Empty, UpdateSampleTimeDefault);
                CorrFactorScopeDefault = driverProfile.GetValue(DriverProgId, CorrFactorScopeProfileName, string.Empty, CorrFactorScopeDefault);
            }
        }

        /// <summary>
        /// Write the device configuration to the  ASCOM  Profile store
        /// </summary>
        internal static void WriteProfile()
        {
            using (Profile driverProfile = new Profile())
            {
                driverProfile.DeviceType = "ObservingConditions";
                driverProfile.WriteValue(DriverProgId, traceStateProfileName, tl.Enabled.ToString());
                driverProfile.WriteValue(DriverProgId, comPortProfileName, comPort.ToString());
                driverProfile.WriteValue(DriverProgId, FWHMpathProfileName, FWHMpathDefault.ToString());
                driverProfile.WriteValue(DriverProgId, BatchfilepathProfileName, BatchfilepathDefault.ToString());
                driverProfile.WriteValue(DriverProgId, BatchfileProfileName, BatchfileDefault.ToString());
                driverProfile.WriteValue(DriverProgId, SensorLogfileProfileName, SensorLogfileDefault.ToString());
                driverProfile.WriteValue(DriverProgId, LogFileStateProfileName, SensorLogEnable.ToString());
                driverProfile.WriteValue(DriverProgId, UpdateSampleTimeProfileName, UpdateSampleTimeDefault.ToString());
                driverProfile.WriteValue(DriverProgId, CorrFactorScopeProfileName, CorrFactorScopeDefault.ToString());  
            }
        }

        /// <summary>
        /// Log helper function that takes identifier and message strings
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        internal static void LogMessage(string identifier, string message)
        {
            tl.LogMessageCrLf(identifier, message);
        }

        /// <summary>
        /// Log helper function that takes formatted strings and arguments
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        internal static void LogMessage(string identifier, string message, params object[] args)
        {
            var msg = string.Format(message, args);
            LogMessage(identifier, msg);
        }
    }
}

