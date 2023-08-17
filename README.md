# Keysight Instrument Plugins
Repository containing source code of cross-platform OpenTAP plugins for multiple Keysight Instruments.
These plugins make it possible for Keysight Test Automation and OpenTAP users to automate creating measurements' setups, performing measurement sweeps, collecting data and publishing results without any external dependency.

*<u>Note:</u> These plugins are auto-generated using XTP tool utilizing native SCPI calls to an instrument.*

**Some key functionalities provided by these plugins are:**

- Connect with various Keysight Instruments.
- Cross-platform enabled supporting Windows and Linux environments.
- View complete heirarchy structure of an Instrument class.
- Cross-platform enabled Help links are provided at each step.
- Verdict handling and Result publishing functionalities are enabled for numeric results.
- Separate instrument driver class is available for custom programming functionality.

**Prerequisites**

- [OpenTAP](https://opentap.io/) or [Keysight Test Automation](https://keysight.com/find/tap)
- [Keysight IO Libraries Suite](https://www.keysight.com/find/iosuite) (2021 or higher)

**Getting OpenTAP**

If you are looking to use OpenTAP, you can get pre-built binaries from [OpenTAP](https://opentap.io/).

Using the OpenTAP CLI, you are now able to download plugin packages directly from the [OpenTAP Package Repository](https://packages.opentap.io/).

To list and install plugin packages, run the following commands:

`tap package list`

`tap package install "<package name>"`

We recommend that you download the Software Development Kit, or simply the Developer's System Community Edition provided by Keysight Technologies. The Developer System is a bundle that contain the SDK as well as a graphical user interface and result viewing capabilities. It can be installed by typing the following command:

`tap package install "Developer's System CE" -y`
