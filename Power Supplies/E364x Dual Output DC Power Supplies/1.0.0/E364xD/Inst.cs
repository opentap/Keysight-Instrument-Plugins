//				Copyright 2023 Keysight Technologies, Inc.
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the
// Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


using OpenTap;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace OpenTap.Plugins.PluginDevelopment
{
    [Display("E364x Dual Output DC Power Supplies", Group: "Keysight Instruments", Description: "Configure E364x Dual Output DC Power Supplies Instrument using a VISA based connection")]
    public class E364xdInstrument : ScpiInstrument
    {
        #region Node Properties
        public Apply Apply { get; private set; }
        public Calibration Calibration { get; private set; }
        public Display Display { get; private set; }
        public Initiate Initiate { get; private set; }
        public Instrument Instrument { get; private set; }
        public Measure Measure { get; private set; }
        public Memory Memory { get; private set; }
        public Output Output { get; private set; }
        public Source Source { get; private set; }
        public Status Status { get; private set; }
        public _System _System { get; private set; }
        public Trigger Trigger { get; private set; }
        #endregion

        #region Constructor
        public E364xdInstrument()
        {
            Name = "E364xD";

            this.Apply = new Apply(this);
            this.Calibration = new Calibration(this);
            this.Display = new Display(this);
            this.Initiate = new Initiate(this);
            this.Instrument = new Instrument(this);
            this.Measure = new Measure(this);
            this.Memory = new Memory(this);
            this.Output = new Output(this);
            this.Source = new Source(this);
            this.Status = new Status(this);
            this._System = new _System(this);
            this.Trigger = new Trigger(this);
        }
        #endregion

        #region Open
        public override void Open()
        {
            base.Open();
        }
        #endregion

        #region Close
        public override void Close()
        {
            base.Close();
        }
        #endregion

    }

    #region Node Classes
    #region Apply class
    public class Apply
    {
        readonly E364xdInstrument e364xd;
        internal Apply(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///This command is combination of VOLTage and CURRent commands.  Query the power supply’s present voltage and current setting values and returns a quoted string.
        ///</summary>
        public void Set(defMinMax voltage, defMinMax current)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":APPL{(voltage.ToString() != "" ? " " + voltage.ToString() : voltage.ToString())}{(voltage.ToString() != "" && current.ToString() != "" ? "," + current.ToString() : (current.ToString() != "" ? " " + current.ToString() : current.ToString()))}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("Set").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command is combination of VOLTage and CURRent commands.  Query the power supply’s present voltage and current setting values and returns a quoted string.
        ///</summary>
        public void Set(string voltage, string current)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":APPL{(voltage != "" ? " " + voltage : voltage)}{(voltage != "" && current != "" ? "," + current : (current != "" ? " " + current : current))}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("Set").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command is combination of VOLTage and CURRent commands.  Query the power supply’s present voltage and current setting values and returns a quoted string.
        ///</summary>
        public string Get()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":APPL?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("Get").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
    }
    #endregion
    #region Calibration class
    public class Calibration
    {
        readonly E364xdInstrument e364xd;
        internal Calibration(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Query the power supply to determine the number of times it has been calibrated.
        ///</summary>
        public int GetCount()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CAL:COUN?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCount").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This command can only be used after calibration is unsecured and the output state is ON.
        ///</summary>
        public void SetCurrentData(double? numericValue)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:CURR{(numericValue.ToString() != "" ? " " + numericValue.ToString() : numericValue.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentData").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command can only be used after calibration is unsecured and the output state is ON.
        ///</summary>
        public void SetCurrentLevel(minMidMax preset)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:CURR:LEV{(preset.ToString() != "" ? " " + preset.ToString() : preset.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentLevel").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command can only be used after calibration is unsecured and the output state is ON.
        ///</summary>
        public void SetCurrentLevel(string preset)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:CURR:LEV{(preset != "" ? " " + preset : preset)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentLevel").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Enter a new security code.
        ///</summary>
        public void SetSecureCode(string code)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:SEC:CODE{(code != "" ? " " + code : code)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSecureCode").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Unsecure or secure the power supply with a security for calibration.  Query the secured state for calibration of the power supply.
        ///</summary>
        public void SetSecureState(bool state, string quotedCode)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:SEC:STAT{(Convert.ToInt32(state).ToString() != "" ? " " + Convert.ToInt32(state).ToString() : Convert.ToInt32(state).ToString())}{(Convert.ToInt32(state).ToString() != "" && quotedCode != "" ? "," + quotedCode : (quotedCode != "" ? " " + quotedCode : quotedCode))}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSecureState").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Unsecure or secure the power supply with a security for calibration.  Query the secured state for calibration of the power supply.
        ///</summary>
        public bool GetSecureState()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CAL:SEC:STAT?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetSecureState").Error("Error: " + _errorCheckVal);
            }
            return responseString != "0" && (responseString == "1" || Convert.ToBoolean(responseString));
        }
        ///<summary>
        ///Record calibration information about your power supply.  Query the calibration message and returns a quoted string.
        ///</summary>
        public void SetString(string quotedString)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:STR{(quotedString != "" ? " " + quotedString : quotedString)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetString").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Record calibration information about your power supply.  Query the calibration message and returns a quoted string.
        ///</summary>
        public string GetString()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CAL:STR?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetString").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
        ///<summary>
        ///This command can only be used after calibration is unsecured and the output state is ON.
        ///</summary>
        public void SetVoltageData(double? numericValue)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:VOLT{(numericValue.ToString() != "" ? " " + numericValue.ToString() : numericValue.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageData").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command can only be used after calibration is unsecured and the output state is ON.
        ///</summary>
        public void SetVoltageLevel(minMidMax preset)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:VOLT:LEV{(preset.ToString() != "" ? " " + preset.ToString() : preset.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageLevel").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command can only be used after calibration is unsecured and the output state is ON.
        ///</summary>
        public void SetVoltageLevel(string preset)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:VOLT:LEV{(preset != "" ? " " + preset : preset)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageLevel").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Calibrate the overvoltage protection circuit of the power supply.
        ///</summary>
        public void SetVoltageProtection()
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CAL:VOLT:PROT"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageProtection").Error("Error Occurred: " + _errorCheckVal);
            }
        }
    }
    #endregion
    #region Display class
    public class Display
    {
        readonly E364xdInstrument e364xd;
        internal Display(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Set the front-panel display mode of the power supply.  Query the state of the display mode.
        ///</summary>
        public void SetWindowMode(mode mode)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":DISP:MODE{(mode.ToString() != "" ? " " + mode.ToString() : mode.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetWindowMode").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the front-panel display mode of the power supply.  Query the state of the display mode.
        ///</summary>
        public void SetWindowMode(string mode)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":DISP:MODE{(mode != "" ? " " + mode : mode)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetWindowMode").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the front-panel display mode of the power supply.  Query the state of the display mode.
        ///</summary>
        public mode GetWindowMode()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":DISP:MODE?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetWindowMode").Error("Error: " + _errorCheckVal);
            }
            return (mode)Enum.Parse(typeof(mode), responseString);
        }
        ///<summary>
        ///Turn the front-panel display off or on.   Query the front-panel display setting.
        ///</summary>
        public void SetWindowState(bool state)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":DISP{(Convert.ToInt32(state).ToString() != "" ? " " + Convert.ToInt32(state).ToString() : Convert.ToInt32(state).ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetWindowState").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Turn the front-panel display off or on.   Query the front-panel display setting.
        ///</summary>
        public bool GetWindowState()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":DISP?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetWindowState").Error("Error: " + _errorCheckVal);
            }
            return responseString != "0" && (responseString == "1" || Convert.ToBoolean(responseString));
        }
        ///<summary>
        ///Clear the message displayed on the front panel.
        ///</summary>
        public void SetWindowTextClear()
        {
            this.e364xd.ScpiCommand(Scpi.Format($":DISP:TEXT:CLE"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetWindowTextClear").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Display a message on the front panel.   Query the message sent to the front panel and returns a quoted string.
        ///</summary>
        public void SetWindowTextData(string quotedString)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":DISP:TEXT{(quotedString != "" ? " " + quotedString : quotedString)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetWindowTextData").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Display a message on the front panel.   Query the message sent to the front panel and returns a quoted string.
        ///</summary>
        public string GetWindowTextData()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":DISP:TEXT?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetWindowTextData").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
    }
    #endregion
    #region Initiate class
    public class Initiate
    {
        readonly E364xdInstrument e364xd;
        internal Initiate(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Cause the trigger system to initiate.
        ///</summary>
        public void SetImmediate()
        {
            this.e364xd.ScpiCommand(Scpi.Format($":INIT"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetImmediate").Error("Error Occurred: " + _errorCheckVal);
            }
        }
    }
    #endregion
    #region Instrument class
    public class Instrument
    {
        readonly E364xdInstrument e364xd;
        internal Instrument(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Select the output to be programmed one of the two outputs by the output identifier.  Return the currently selected output by the INSTrument{:SELect] or INSTrument:NSELect command.
        ///</summary>
        public void SetSelect(channel select)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":INST{(select.ToString() != "" ? " " + select.ToString() : select.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSelect").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Select the output to be programmed one of the two outputs by the output identifier.  Return the currently selected output by the INSTrument{:SELect] or INSTrument:NSELect command.
        ///</summary>
        public void SetSelect(string select)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":INST{(select != "" ? " " + select : select)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSelect").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Select the output to be programmed one of the two outputs by the output identifier.  Return the currently selected output by the INSTrument{:SELect] or INSTrument:NSELect command.
        ///</summary>
        public channel GetSelect()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":INST?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetSelect").Error("Error: " + _errorCheckVal);
            }
            return (channel)Enum.Parse(typeof(channel), responseString);
        }
        ///<summary>
        ///Select the output to be programmed one of the two outputs by a numeric value instead of the output identifier used in the INSTrument:NSELect or INSTrument[:SELect] command.  Return the currently selected output by the INSTrument[SELect]or INSTrument[SELect] command.
        ///</summary>
        public void SetNselect(int? nselect)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":INST:NSEL{(nselect.ToString() != "" ? " " + nselect.ToString() : nselect.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetNselect").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Select the output to be programmed one of the two outputs by a numeric value instead of the output identifier used in the INSTrument:NSELect or INSTrument[:SELect] command.  Return the currently selected output by the INSTrument[SELect]or INSTrument[SELect] command.
        ///</summary>
        public int GetNselect()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":INST:NSEL?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetNselect").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///Enable or disable a coupling between two logical outputs of the power supply.  Query the output coupling state of the power supply.
        ///</summary>
        public void SetCoupleTrigger(bool state)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":INST:COUP{(Convert.ToInt32(state).ToString() != "" ? " " + Convert.ToInt32(state).ToString() : Convert.ToInt32(state).ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCoupleTrigger").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Enable or disable a coupling between two logical outputs of the power supply.  Query the output coupling state of the power supply.
        ///</summary>
        public int GetCoupleTrigger()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":INST:COUP?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCoupleTrigger").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
    }
    #endregion
    #region Measure class
    public class Measure
    {
        readonly E364xdInstrument e364xd;
        internal Measure(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Query the current measured across the current sense resistor inside the power supply.
        ///</summary>
        public double GetScalarCurrentDc()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":MEAS:CURR?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetScalarCurrentDc").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Query the voltage measured at the sense terminals of the power supply.
        ///</summary>
        public double GetScalarVoltageDc()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":MEAS?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetScalarVoltageDc").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
    }
    #endregion
    #region Memory class
    public class Memory
    {
        readonly E364xdInstrument e364xd;
        internal Memory(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Assign a name to the specified storage location.
        ///</summary>
        public void SetStateName(int? name, string quotedName)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":MEM:STAT:NAME{(name.ToString() != "" ? " " + name.ToString() : name.ToString())}{(name.ToString() != "" && quotedName != "" ? "," + quotedName : (quotedName != "" ? " " + quotedName : quotedName))}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetStateName").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Assign a name to the specified storage location.
        ///</summary>
        public string GetStateName(int? name)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":MEM:STAT:NAME?{(name.ToString() != "" ? " " + name.ToString() : name.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetStateName").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
    }
    #endregion
    #region Output class
    public class Output
    {
        readonly E364xdInstrument e364xd;
        internal Output(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Enable or disable the outputs of the power supply.   Query the output state of the power supply.
        ///</summary>
        public void SetState(bool state)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":OUTP{(Convert.ToInt32(state).ToString() != "" ? " " + Convert.ToInt32(state).ToString() : Convert.ToInt32(state).ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetState").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Enable or disable the outputs of the power supply.   Query the output state of the power supply.
        ///</summary>
        public bool GetState()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":OUTP?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetState").Error("Error: " + _errorCheckVal);
            }
            return responseString != "0" && (responseString == "1" || Convert.ToBoolean(responseString));
        }
        ///<summary>
        ///Set the state of two TTL signals on the RS-232 connector pin 1 and pin 9. These signals are intended for use with an external relay and relay driver. At *RST, the OUTPUT:RELay state is OFF.    Query the state of the TTL relay logic signals.
        ///</summary>
        public void SetRelayState(bool state)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":OUTP:REL{(Convert.ToInt32(state).ToString() != "" ? " " + Convert.ToInt32(state).ToString() : Convert.ToInt32(state).ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetRelayState").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the state of two TTL signals on the RS-232 connector pin 1 and pin 9. These signals are intended for use with an external relay and relay driver. At *RST, the OUTPUT:RELay state is OFF.    Query the state of the TTL relay logic signals.
        ///</summary>
        public bool GetRelayState()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":OUTP:REL?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetRelayState").Error("Error: " + _errorCheckVal);
            }
            return responseString != "0" && (responseString == "1" || Convert.ToBoolean(responseString));
        }
        ///<summary>
        ///Enable or disable the power supply to operate in the track mode.   Query the tracking mode state of the power supply.
        ///</summary>
        public void SetTrackState(bool state)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":OUTP:TRAC{(Convert.ToInt32(state).ToString() != "" ? " " + Convert.ToInt32(state).ToString() : Convert.ToInt32(state).ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetTrackState").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Enable or disable the power supply to operate in the track mode.   Query the tracking mode state of the power supply.
        ///</summary>
        public bool GetTrackState()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":OUTP:TRAC?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetTrackState").Error("Error: " + _errorCheckVal);
            }
            return responseString != "0" && (responseString == "1" || Convert.ToBoolean(responseString));
        }
    }
    #endregion
    #region Source class
    public class Source
    {
        readonly E364xdInstrument e364xd;
        internal Source(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Program the immediate current level of the power supply.   Return the presently programmed current level of the power supply.
        ///</summary>
        public void SetCurrentLevelImmediateAmplitude(minMaxUpDown current)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CURR{(current.ToString() != "" ? " " + current.ToString() : current.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentLevelImmediateAmplitude").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Program the immediate current level of the power supply.   Return the presently programmed current level of the power supply.
        ///</summary>
        public void SetCurrentLevelImmediateAmplitude(string current)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CURR{(current != "" ? " " + current : current)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentLevelImmediateAmplitude").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Program the immediate current level of the power supply.   Return the presently programmed current level of the power supply.
        ///</summary>
        public double GetCurrentLevelImmediateAmplitude(minMax preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CURR?{(preset.ToString() != "" ? " " + preset.ToString() : preset.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCurrentLevelImmediateAmplitude").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Program the immediate current level of the power supply.   Return the presently programmed current level of the power supply.
        ///</summary>
        public double GetCurrentLevelImmediateAmplitude(string preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CURR?{(preset != "" ? " " + preset : preset)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCurrentLevelImmediateAmplitude").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Set the step size for current programming with the CURRent UPand CURRentDOWN commands.   Return the value of the step size currently specified.
        ///</summary>
        public void SetCurrentLevelImmediateStepIncrement(_default numericValue)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CURR:STEP{(numericValue.ToString() != "" ? " " + numericValue.ToString() : numericValue.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentLevelImmediateStepIncrement").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the step size for current programming with the CURRent UPand CURRentDOWN commands.   Return the value of the step size currently specified.
        ///</summary>
        public void SetCurrentLevelImmediateStepIncrement(string numericValue)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CURR:STEP{(numericValue != "" ? " " + numericValue : numericValue)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentLevelImmediateStepIncrement").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the step size for current programming with the CURRent UPand CURRentDOWN commands.   Return the value of the step size currently specified.
        ///</summary>
        public double GetCurrentLevelImmediateStepIncrement(_default _default)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CURR:STEP?{(_default.ToString() != "" ? " " + _default.ToString() : _default.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCurrentLevelImmediateStepIncrement").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Set the step size for current programming with the CURRent UPand CURRentDOWN commands.   Return the value of the step size currently specified.
        ///</summary>
        public double GetCurrentLevelImmediateStepIncrement(string _default)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CURR:STEP?{(_default != "" ? " " + _default : _default)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCurrentLevelImmediateStepIncrement").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Program the pending triggered current level.   Query the triggered current level presently programmed.
        ///</summary>
        public void SetCurrentLevelTriggeredAmplitude(minMax current)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CURR:TRIG{(current.ToString() != "" ? " " + current.ToString() : current.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentLevelTriggeredAmplitude").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Program the pending triggered current level.   Query the triggered current level presently programmed.
        ///</summary>
        public void SetCurrentLevelTriggeredAmplitude(string current)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":CURR:TRIG{(current != "" ? " " + current : current)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCurrentLevelTriggeredAmplitude").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Program the pending triggered current level.   Query the triggered current level presently programmed.
        ///</summary>
        public double GetCurrentLevelTriggeredAmplitude(minMax preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CURR:TRIG?{(preset.ToString() != "" ? " " + preset.ToString() : preset.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCurrentLevelTriggeredAmplitude").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Program the pending triggered current level.   Query the triggered current level presently programmed.
        ///</summary>
        public double GetCurrentLevelTriggeredAmplitude(string preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":CURR:TRIG?{(preset != "" ? " " + preset : preset)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCurrentLevelTriggeredAmplitude").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Select an output range to be programmed by the identifier.  Query the currently selected range.
        ///</summary>
        public void SetVoltageRange(output range)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:RANG{(range.ToString() != "" ? " " + range.ToString() : range.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageRange").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Select an output range to be programmed by the identifier.  Query the currently selected range.
        ///</summary>
        public void SetVoltageRange(string range)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:RANG{(range != "" ? " " + range : range)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageRange").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Select an output range to be programmed by the identifier.  Query the currently selected range.
        ///</summary>
        public output GetVoltageRange()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:RANG?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageRange").Error("Error: " + _errorCheckVal);
            }
            return (output)Enum.Parse(typeof(output), responseString);
        }
        ///<summary>
        ///Cause the overvoltage protection circuit to be cleared.
        ///</summary>
        public void SetVoltageProtectionClear()
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:PROT:CLE"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageProtectionClear").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the voltage level at which the overvoltage protection (OVP) circuit will trip.   Query the overvoltage protection trip level presently programmed.
        ///</summary>
        public void SetVoltageProtectionLevel(minMax voltage)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:PROT{(voltage.ToString() != "" ? " " + voltage.ToString() : voltage.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageProtectionLevel").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the voltage level at which the overvoltage protection (OVP) circuit will trip.   Query the overvoltage protection trip level presently programmed.
        ///</summary>
        public void SetVoltageProtectionLevel(string voltage)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:PROT{(voltage != "" ? " " + voltage : voltage)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageProtectionLevel").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the voltage level at which the overvoltage protection (OVP) circuit will trip.   Query the overvoltage protection trip level presently programmed.
        ///</summary>
        public double GetVoltageProtectionLevel(minMax preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:PROT?{(preset.ToString() != "" ? " " + preset.ToString() : preset.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageProtectionLevel").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Set the voltage level at which the overvoltage protection (OVP) circuit will trip.   Query the overvoltage protection trip level presently programmed.
        ///</summary>
        public double GetVoltageProtectionLevel(string preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:PROT?{(preset != "" ? " " + preset : preset)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageProtectionLevel").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Enable or disable the overvoltage protection function.  Query the state of the overvoltage protection function.
        ///</summary>
        public void SetVoltageProtectionState(bool state)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:PROT:STAT{(Convert.ToInt32(state).ToString() != "" ? " " + Convert.ToInt32(state).ToString() : Convert.ToInt32(state).ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageProtectionState").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Enable or disable the overvoltage protection function.  Query the state of the overvoltage protection function.
        ///</summary>
        public bool GetVoltageProtectionState()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:PROT:STAT?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageProtectionState").Error("Error: " + _errorCheckVal);
            }
            return responseString != "0" && (responseString == "1" || Convert.ToBoolean(responseString));
        }
        ///<summary>
        ///Return a ‘‘1’’ if the overvoltage protection circuit is tripped and not cleared or a ‘‘0’’ if not tripped.
        ///</summary>
        public bool GetVoltageProtectionTripped()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:PROT:TRIP?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageProtectionTripped").Error("Error: " + _errorCheckVal);
            }
            return responseString != "0" && (responseString == "1" || Convert.ToBoolean(responseString));
        }
        ///<summary>
        ///Program the immediate voltage level of the power supply.   Query the presently programmed voltage level of the power supply.
        ///</summary>
        public void SetVoltageLevelImmediateAmplitude(minMaxUpDown voltage)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT{(voltage.ToString() != "" ? " " + voltage.ToString() : voltage.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageLevelImmediateAmplitude").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Program the immediate voltage level of the power supply.   Query the presently programmed voltage level of the power supply.
        ///</summary>
        public void SetVoltageLevelImmediateAmplitude(string voltage)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT{(voltage != "" ? " " + voltage : voltage)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageLevelImmediateAmplitude").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Program the immediate voltage level of the power supply.   Query the presently programmed voltage level of the power supply.
        ///</summary>
        public double GetVoltageLevelImmediateAmplitude(minMax preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT?{(preset.ToString() != "" ? " " + preset.ToString() : preset.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageLevelImmediateAmplitude").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Program the immediate voltage level of the power supply.   Query the presently programmed voltage level of the power supply.
        ///</summary>
        public double GetVoltageLevelImmediateAmplitude(string preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT?{(preset != "" ? " " + preset : preset)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageLevelImmediateAmplitude").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Set the step size for voltage programming with the VOLT UP and VOLT DOWN commands.  Return the value of the step size currently specified.
        ///</summary>
        public void SetVoltageLevelImmediateStepIncrement(_default numericValue)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:STEP{(numericValue.ToString() != "" ? " " + numericValue.ToString() : numericValue.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageLevelImmediateStepIncrement").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the step size for voltage programming with the VOLT UP and VOLT DOWN commands.  Return the value of the step size currently specified.
        ///</summary>
        public void SetVoltageLevelImmediateStepIncrement(string numericValue)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:STEP{(numericValue != "" ? " " + numericValue : numericValue)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageLevelImmediateStepIncrement").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the step size for voltage programming with the VOLT UP and VOLT DOWN commands.  Return the value of the step size currently specified.
        ///</summary>
        public double GetVoltageLevelImmediateStepIncrement(_default _default)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:STEP?{(_default.ToString() != "" ? " " + _default.ToString() : _default.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageLevelImmediateStepIncrement").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Set the step size for voltage programming with the VOLT UP and VOLT DOWN commands.  Return the value of the step size currently specified.
        ///</summary>
        public double GetVoltageLevelImmediateStepIncrement(string _default)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:STEP?{(_default != "" ? " " + _default : _default)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageLevelImmediateStepIncrement").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Program the pending triggered voltage level.   Query the triggered voltage level presently programmed.
        ///</summary>
        public void SetVoltageLevelTriggeredAmplitude(minMax voltage)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:TRIG{(voltage.ToString() != "" ? " " + voltage.ToString() : voltage.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageLevelTriggeredAmplitude").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Program the pending triggered voltage level.   Query the triggered voltage level presently programmed.
        ///</summary>
        public void SetVoltageLevelTriggeredAmplitude(string voltage)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":VOLT:TRIG{(voltage != "" ? " " + voltage : voltage)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetVoltageLevelTriggeredAmplitude").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Program the pending triggered voltage level.   Query the triggered voltage level presently programmed.
        ///</summary>
        public double GetVoltageLevelTriggeredAmplitude(minMax preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:TRIG?{(preset.ToString() != "" ? " " + preset.ToString() : preset.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageLevelTriggeredAmplitude").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Program the pending triggered voltage level.   Query the triggered voltage level presently programmed.
        ///</summary>
        public double GetVoltageLevelTriggeredAmplitude(string preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":VOLT:TRIG?{(preset != "" ? " " + preset : preset)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVoltageLevelTriggeredAmplitude").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
    }
    #endregion
    #region Status class
    public class Status
    {
        readonly E364xdInstrument e364xd;
        internal Status(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Enable bits in the Questionable Status Enable register.   Query the Questionable Status Enable register.
        ///</summary>
        public void SetQuestionableEnable(int? enable)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":STAT:QUES:ENAB{(enable.ToString() != "" ? " " + enable.ToString() : enable.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetQuestionableEnable").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Enable bits in the Questionable Status Enable register.   Query the Questionable Status Enable register.
        ///</summary>
        public int GetQuestionableEnable()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":STAT:QUES:ENAB?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetQuestionableEnable").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///Query the Questionable Status Event register.
        ///</summary>
        public int GetQuestionableEvent()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":STAT:QUES?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetQuestionableEvent").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///Query the Questionable Instrument Event register.
        ///</summary>
        public int GetQuestionableInstrumentEvent()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":STAT:QUES:INST?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetQuestionableInstrumentEvent").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///Set the value of the Questionable Instrument Enable register.  Return the value of the Questionable Instrument Enable register.
        ///</summary>
        public void SetQuestionableInstrumentEnable(int? enableValue)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":STAT:QUES:INST:ENAB{(enableValue.ToString() != "" ? " " + enableValue.ToString() : enableValue.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetQuestionableInstrumentEnable").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the value of the Questionable Instrument Enable register.  Return the value of the Questionable Instrument Enable register.
        ///</summary>
        public int GetQuestionableInstrumentEnable()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":STAT:QUES:INST:ENAB?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetQuestionableInstrumentEnable").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///Return the CV or CC condition of the specified instrument.
        ///</summary>
        public int GetQuestionableInstrumentIsummaryCondition(int? ISUMSuffix)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":STAT:QUES:INST:ISUM{ISUMSuffix}:COND?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetQuestionableInstrumentIsummaryCondition").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///Return the value of the Questionable Instrument Isummary Event register for a specific output of the two-output power supply.
        ///</summary>
        public int GetQuestionableInstrumentIsummaryEvent(int? ISUMSuffix)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":STAT:QUES:INST:ISUM{ISUMSuffix}?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetQuestionableInstrumentIsummaryEvent").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///Set the value of the Questionable Instrument Isummary Enable register for a specific output of the two-output power supply.   This query returns the value of the Questionable Instrument Isummary Enable register.
        ///</summary>
        public void SetQuestionableInstrumentIsummaryEnable(int? ISUMSuffix, int? enableValue)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":STAT:QUES:INST:ISUM{ISUMSuffix}:ENAB{(enableValue.ToString() != "" ? " " + enableValue.ToString() : enableValue.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetQuestionableInstrumentIsummaryEnable").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the value of the Questionable Instrument Isummary Enable register for a specific output of the two-output power supply.   This query returns the value of the Questionable Instrument Isummary Enable register.
        ///</summary>
        public int GetQuestionableInstrumentIsummaryEnable(int? ISUMSuffix)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":STAT:QUES:INST:ISUM{ISUMSuffix}:ENAB?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetQuestionableInstrumentIsummaryEnable").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
    }
    #endregion
    #region System class
    public class _System
    {
        readonly E364xdInstrument e364xd;
        internal _System(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Issue a single beep immediately.
        ///</summary>
        public void SetBeeperImmediate()
        {
            this.e364xd.ScpiCommand(Scpi.Format($":SYST:BEEP"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetBeeperImmediate").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Query the power supply’s error queue.
        ///</summary>
        public (int, string) GetError()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":SYST:ERR?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetError").Error("Error: " + _errorCheckVal);
            }
            string[] resArr = responseString.Replace("(", "").Replace(")", "").Replace("@", "").Split(',');
            return (Convert.ToInt32(resArr[0]), resArr[1]);
        }
        ///<summary>
        ///Select the remote interface.
        ///</summary>
        public void SetInterface(gpibRs232 _interface)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":SYST:INT{(_interface.ToString() != "" ? " " + _interface.ToString() : _interface.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetInterface").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Select the remote interface.
        ///</summary>
        public void SetInterface(string _interface)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":SYST:INT{(_interface != "" ? " " + _interface : _interface)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetInterface").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Place the power supply in the local mode during RS-232 operation.
        ///</summary>
        public void SetLocal()
        {
            this.e364xd.ScpiCommand(Scpi.Format($":SYST:LOC"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetLocal").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Place the power supply in the remote mode for RS-232 operation.
        ///</summary>
        public void SetRemote()
        {
            this.e364xd.ScpiCommand(Scpi.Format($":SYST:REM"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetRemote").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Place the power supply in the remote mode for RS-232 operation.
        ///</summary>
        public void SetRwlock()
        {
            this.e364xd.ScpiCommand(Scpi.Format($":SYST:RWL"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetRwlock").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Query the power supply to determine the present SCPI version.
        ///</summary>
        public string GetVersion()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":SYST:VERS?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetVersion").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
    }
    #endregion
    #region Trigger class
    public class Trigger
    {
        readonly E364xdInstrument e364xd;
        internal Trigger(E364xdInstrument e364xd)
        {
            this.e364xd = e364xd;
        }

        ///<summary>
        ///Set the time delay between the detection of an event on the specified trigger source and the start of any corresponding trigger action on the power supply output.  Query the trigger delay.
        ///</summary>
        public void SetSequenceDelay(minMax seconds)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":TRIG:DEL{(seconds.ToString() != "" ? " " + seconds.ToString() : seconds.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSequenceDelay").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the time delay between the detection of an event on the specified trigger source and the start of any corresponding trigger action on the power supply output.  Query the trigger delay.
        ///</summary>
        public void SetSequenceDelay(string seconds)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":TRIG:DEL{(seconds != "" ? " " + seconds : seconds)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSequenceDelay").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Set the time delay between the detection of an event on the specified trigger source and the start of any corresponding trigger action on the power supply output.  Query the trigger delay.
        ///</summary>
        public double GetSequenceDelay(minMax preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":TRIG:DEL?{(preset.ToString() != "" ? " " + preset.ToString() : preset.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetSequenceDelay").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Set the time delay between the detection of an event on the specified trigger source and the start of any corresponding trigger action on the power supply output.  Query the trigger delay.
        ///</summary>
        public double GetSequenceDelay(string preset)
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":TRIG:DEL?{(preset != "" ? " " + preset : preset)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetSequenceDelay").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToDouble(responseString);
        }
        ///<summary>
        ///Select the source from which the power supply will accept a trigger.  Query the present trigger source.
        ///</summary>
        public void SetSequenceSource(busImmediate source)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":TRIG:SOUR{(source.ToString() != "" ? " " + source.ToString() : source.ToString())}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSequenceSource").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Select the source from which the power supply will accept a trigger.  Query the present trigger source.
        ///</summary>
        public void SetSequenceSource(string source)
        {
            this.e364xd.ScpiCommand(Scpi.Format($":TRIG:SOUR{(source != "" ? " " + source : source)}"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSequenceSource").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///Select the source from which the power supply will accept a trigger.  Query the present trigger source.
        ///</summary>
        public busImmediate GetSequenceSource()
        {
            string responseString = this.e364xd.ScpiQuery<string>(Scpi.Format($":TRIG:SOUR?"));
            string _errorCheckVal = this.e364xd.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetSequenceSource").Error("Error: " + _errorCheckVal);
            }
            return (busImmediate)Enum.Parse(typeof(busImmediate), responseString);
        }
    }
    #endregion
    #endregion
}