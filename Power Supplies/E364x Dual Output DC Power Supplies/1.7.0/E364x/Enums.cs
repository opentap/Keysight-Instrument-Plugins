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


namespace OpenTap.Plugins.PluginDevelopment
{
    #region output
    public enum output
    {
        /// <summary>
        /// P8V value
        /// </summary>
        P8V,
        /// <summary>
        /// P20V value
        /// </summary>
        P20V,
        /// <summary>
        /// P35V value
        /// </summary>
        P35V,
        /// <summary>
        /// P60V value
        /// </summary>
        P60V,
        /// <summary>
        /// LOW value
        /// </summary>
        LOW,
        /// <summary>
        /// HIGH value
        /// </summary>
        HIGH
    }
    #endregion

    #region defMinMax
    public enum defMinMax
    {
        /// <summary>
        /// DEFault value
        /// </summary>
        DEF,
        /// <summary>
        /// MINimum value
        /// </summary>
        MIN,
        /// <summary>
        /// MAXimum value
        /// </summary>
        MAX
    }
    #endregion

    #region minMax
    public enum minMax
    {
        /// <summary>
        /// MINimum value
        /// </summary>
        MIN,
        /// <summary>
        /// MAXimum value
        /// </summary>
        MAX
    }
    #endregion

    #region mode
    public enum mode
    {
        /// <summary>
        /// VV value
        /// </summary>
        VV,
        /// <summary>
        /// VI value
        /// </summary>
        VI,
        /// <summary>
        /// II value
        /// </summary>
        II
    }
    #endregion

    #region gpibRs232
    public enum gpibRs232
    {
        /// <summary>
        /// GPIB value
        /// </summary>
        GPIB,
        /// <summary>
        /// RS232 value
        /// </summary>
        RS232
    }
    #endregion

    #region minMidMax
    public enum minMidMax
    {
        /// <summary>
        /// MINimum value
        /// </summary>
        MIN,
        /// <summary>
        /// MIDdle value
        /// </summary>
        MID,
        /// <summary>
        /// MAXimum value
        /// </summary>
        MAX
    }
    #endregion

    #region minMaxUpDown
    public enum minMaxUpDown
    {
        /// <summary>
        /// MINimum value
        /// </summary>
        MIN,
        /// <summary>
        /// MAXimum value
        /// </summary>
        MAX,
        /// <summary>
        /// DOWN value
        /// </summary>
        DOWN
    }
    #endregion

    #region channel
    public enum channel
    {
        /// <summary>
        /// OUTput1 value
        /// </summary>
        OUTP1,
        /// <summary>
        /// OUTput2 value
        /// </summary>
        OUTP2
    }
    #endregion

    #region default
    public enum _default
    {
        /// <summary>
        /// DEFault value
        /// </summary>
        DEF
    }
    #endregion

    #region busImmediate
    public enum busImmediate
    {
        /// <summary>
        /// BUS value
        /// </summary>
        BUS,
        /// <summary>
        /// IMMediate value
        /// </summary>
        IMM
    }
    #endregion

    #region boolean
    public enum boolean
    {
        /// <summary>
        /// OFF value
        /// </summary>
        OFF,
        /// <summary>
        /// ON value
        /// </summary>
        ON
    }
    #endregion

}