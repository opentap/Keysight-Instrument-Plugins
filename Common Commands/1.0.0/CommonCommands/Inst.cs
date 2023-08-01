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
    #region Common Commands
    public class CommonCommands
    {
        readonly ScpiInstrument commoncommands;
        public CommonCommands(ScpiInstrument commoncommands)
        {
            this.commoncommands = commoncommands;
        }

        ///<summary>
        ///This command, in conjunction with the Address Set protocol, allows the controller to detect all address-configurable devices (that is, devices that implement this command) and assign an IEEE 488.1 address to each of those devices.
        ///</summary>
        public void SetAad()
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*AAD"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetAad").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Calibration query causes a device to perform an internal self-calibration and generate a response that indicates whether or not the device completed the self-calibration without error.
        ///</summary>
        public int GetCal()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*CAL?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetCal").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This command clears the event registers in all register groups. This command also clears the Error queue.
        ///</summary>
        public void SetCls()
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*CLS"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetCls").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Define Device Trigger command stores a command sequence that is executed when a group execute trigger (GET), IEEE 488.1 interface message, or *TRG common command is received.
        ///</summary>
        public void SetDdtString(string _string)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*DDT{(_string != "" ? " " + _string : _string)}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetDdtString").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Define Device Trigger command stores a command sequence that is executed when a group execute trigger (GET), IEEE 488.1 interface message, or *TRG common command is received.
        ///</summary>
        public void SetDdtBlock(string block_data)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*DDT{(block_data != "" ? " " + block_data : block_data)}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetDdtBlock").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Define Device Trigger command stores a command sequence that is executed when a group execute trigger (GET), IEEE 488.1 interface message, or *TRG common command is received.
        ///</summary>
        public byte[] GetDdt()
        {
            return this.commoncommands.ScpiQueryBlock<byte>(Scpi.Format($"*DDT?"));
        }
        ///<summary>
        ///The Disable Listener Function command causes a device to cease being a listener (change to L0 subset).
        ///</summary>
        public void SetDlf()
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*DLF"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetDlf").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Define Macro command allows the programmer to assign a sequence of zero or more program message unit elements to a macro label.
        ///</summary>
        public void SetDmcBlock(string _string, string block_data)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*DMC{(_string != "" ? " " + _string : _string)}{(_string != "" && block_data != "" ? "," + block_data : (block_data != "" ? " " + block_data : block_data))}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetDmcBlock").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Define Macro command allows the programmer to assign a sequence of zero or more program message unit elements to a macro label.
        ///</summary>
        public void SetDmcString(string _string, string macro_string)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*DMC{(_string != "" ? " " + _string : _string)}{(_string != "" && macro_string != "" ? "," + macro_string : (macro_string != "" ? " " + macro_string : macro_string))}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetDmcString").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Enable Macro command enables and disables expansion of macros.
        ///</summary>
        public void SetEmc(int? integer)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*EMC{(integer.ToString() != "" ? " " + integer.ToString() : integer.ToString())}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetEmc").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Enable Macro command enables and disables expansion of macros.
        ///</summary>
        public int GetEmc()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*EMC?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetEmc").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This command enables bits in the enable register for the Standard Event Register group.
        ///</summary>
        public void SetEse(int? enable_value)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*ESE{(enable_value.ToString() != "" ? " " + enable_value.ToString() : enable_value.ToString())}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetEse").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command enables bits in the enable register for the Standard Event Register group.
        ///</summary>
        public int GetEse()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*ESE?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetEse").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This command queries the event register for the Standard Event Register group.
        ///</summary>
        public int GetEsr()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*ESR?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetEsr").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///The Get Macro Contents query allows the current definition of a macro to be retrieved from a device.
        ///</summary>
        public byte[] GetGmc(string label)
        {
            return this.commoncommands.ScpiQueryBlock<byte>(Scpi.Format($"*GMC?{(label != "" ? " " + label : label)}"));
        }
        ///<summary>
        ///This command reads the instrument's (mainframe) identification string which contains comma-separated fields.
        ///</summary>
        public string GetIdn()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*IDN?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetIdn").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
        ///<summary>
        ///The Individual Status query allows the programmer to read the current state of the IEEE 488.1 defined “ist” local message in the device.
        ///</summary>
        public int GetIst()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*IST?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetIst").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This query returns the currently defined macro labels.
        ///</summary>
        public string GetLmc()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*LMC?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetLmc").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
        ///<summary>
        ///This command returns the learn string, which is an ASCII string of SCPI commands.
        ///</summary>
        public string GetLrn()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*LRN?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetLrn").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
        ///<summary>
        ///This command sets the ”Operation Complete” bit (bit 0) in the Standard Event register at the completion of the current operation.  This command returns ”1” to the output buffer at the completion of the current operation.
        ///</summary>
        public void SetOpc()
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*OPC"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetOpc").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command sets the ”Operation Complete” bit (bit 0) in the Standard Event register at the completion of the current operation.  This command returns ”1” to the output buffer at the completion of the current operation.
        ///</summary>
        public int GetOpc()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*OPC?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetOpc").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///The Option Identification query is for identifying reportable device options over the system interface.
        ///</summary>
        public string GetOpt()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*OPT?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetOpt").Error("Error: " + _errorCheckVal);
            }
            return responseString;
        }
        ///<summary>
        ///The Pass Control Back command is used by a controller to tell a device, being a potential controller, to which address the control is to be passed back when the device (acting as a controller) sends the IEEE 488.1 interface message, take control (TCT).
        ///</summary>
        public void SetPcb(int? integer1, int? integer2)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*PCB{(integer1.ToString() != "" ? " " + integer1.ToString() : integer1.ToString())}{(integer1.ToString() != "" && integer2.ToString() != "" ? "," + integer2.ToString() : (integer2.ToString() != "" ? " " + integer2.ToString() : integer2.ToString()))}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetPcb").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Purge Macros command causes the device to delete all macros that may have been previously defined using the *DMC command.
        ///</summary>
        public void SetPmc()
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*PMC"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetPmc").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Parallel Poll Enable Register command sets the Parallel Poll Enable Register bits.  The Parallel Poll Enable Register query allows the programmer to determine the current contents of the Parallel Poll Enable Register.
        ///</summary>
        public void SetPre(int? integer)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*PRE{(integer.ToString() != "" ? " " + integer.ToString() : integer.ToString())}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetPre").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Parallel Poll Enable Register command sets the Parallel Poll Enable Register bits.  The Parallel Poll Enable Register query allows the programmer to determine the current contents of the Parallel Poll Enable Register.
        ///</summary>
        public int GetPre()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*PRE?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetPre").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This command enables or disables the clearing of certain enable registers at power on.
        ///</summary>
        public void SetPsc(int? psc)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*PSC{(psc.ToString() != "" ? " " + psc.ToString() : psc.ToString())}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetPsc").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command enables or disables the clearing of certain enable registers at power on.
        ///</summary>
        public int GetPsc()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*PSC?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetPsc").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///The Protected User Data command stores data unique to the device such as calibration date, usage time, environmental conditions, and inventory control numbers.  The Protected User Data query allows the programmer to retrieve the contents of the *PUD storage area.
        ///</summary>
        public void SetPudString(string _string)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*PUD{(_string != "" ? " " + _string : _string)}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetPudString").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Protected User Data command stores data unique to the device such as calibration date, usage time, environmental conditions, and inventory control numbers.  The Protected User Data query allows the programmer to retrieve the contents of the *PUD storage area.
        ///</summary>
        public void SetPudBlock(byte[] block_data)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*PUD{(string.Join(",", block_data) != "" ? " " + string.Join(",", block_data) : string.Join(",", block_data))}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetPudBlock").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Protected User Data command stores data unique to the device such as calibration date, usage time, environmental conditions, and inventory control numbers.  The Protected User Data query allows the programmer to retrieve the contents of the *PUD storage area.
        ///</summary>
        public byte[] GetPud()
        {
            return this.commoncommands.ScpiQueryBlock<byte>(Scpi.Format($"*PUD?"));
        }
        ///<summary>
        ///This command recalls the instrument state stored in the specified storage location.
        ///</summary>
        public void SetRcl(int? rcl)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*RCL{(rcl.ToString() != "" ? " " + rcl.ToString() : rcl.ToString())}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetRcl").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Resource Description Transfer command allows a Resource Description to be stored in a device.  The Resource Description Transfer query allows a Resource Description to be retrieved from a device. The Resource Description may be memory or in a read-write memory settable by the *RDT command.
        ///</summary>
        public void SetRdtString(string _string)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*RDT{(_string != "" ? " " + _string : _string)}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetRdtString").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Resource Description Transfer command allows a Resource Description to be stored in a device.  The Resource Description Transfer query allows a Resource Description to be retrieved from a device. The Resource Description may be memory or in a read-write memory settable by the *RDT command.
        ///</summary>
        public void SetRdtBlock(byte[] block_data)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*RDT{(string.Join(",", block_data) != "" ? " " + string.Join(",", block_data) : string.Join(",", block_data))}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetRdtBlock").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Resource Description Transfer command allows a Resource Description to be stored in a device.  The Resource Description Transfer query allows a Resource Description to be retrieved from a device. The Resource Description may be memory or in a read-write memory settable by the *RDT command.
        ///</summary>
        public byte[] GetRdt()
        {
            return this.commoncommands.ScpiQueryBlock<byte>(Scpi.Format($"*RDT?"));
        }
        ///<summary>
        ///This command resets the multimeter to the Factory configuration.
        ///</summary>
        public void SetRst()
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*RST"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetRst").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command stores (saves) the current instrument state in the specified storage location.
        ///</summary>
        public void SetSav(int? sav)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*SAV{(sav.ToString() != "" ? " " + sav.ToString() : sav.ToString())}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSav").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command enables bits in the enable register for the Status Byte Register group.
        ///</summary>
        public void SetSre(int? enable_value)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*SRE{(enable_value.ToString() != "" ? " " + enable_value.ToString() : enable_value.ToString())}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSre").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command enables bits in the enable register for the Status Byte Register group.
        ///</summary>
        public int GetSre()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*SRE?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetSre").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This command queries the condition register for the Status Byte Register group.
        ///</summary>
        public int GetStb()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*STB?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetStb").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This command is used to trigger the instrument from the remote interface.
        ///</summary>
        public void SetTrg()
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*TRG"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetTrg").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///This command performs a complete self-test of the instrument and returns a pass/fail indication.
        ///</summary>
        public int GetTst()
        {
            string responseString = this.commoncommands.ScpiQuery<string>(Scpi.Format($"*TST?"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("GetTst").Error("Error: " + _errorCheckVal);
            }
            return Convert.ToInt32(responseString);
        }
        ///<summary>
        ///This command configures the instrument's output buffer to wait for all pending operations to complete before executing any additional commands over the interface.
        ///</summary>
        public void SetWai()
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*WAI"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetWai").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Remove Individual Macro command removes a single macro definition from the device.
        ///</summary>
        public void SetRmc(string label)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*RMC{(label != "" ? " " + label : label)}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetRmc").Error("Error Occurred: " + _errorCheckVal);
            }
        }
        ///<summary>
        ///The Save Default Device Settings command initializes the contents of a save/recall register.
        ///</summary>
        public void SetSds(int? integer)
        {
            this.commoncommands.ScpiCommand(Scpi.Format($"*SDS{(integer.ToString() != "" ? " " + integer.ToString() : integer.ToString())}"));
            string _errorCheckVal = this.commoncommands.ScpiQuery<string>(Scpi.Format("SYST:ERR?"));
            if (!_errorCheckVal.Replace(" ", "").ToUpper().Contains("NOERROR"))
            {
                Log.CreateSource("SetSds").Error("Error Occurred: " + _errorCheckVal);
            }
        }
    }
    #endregion
}