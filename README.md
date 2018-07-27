# testwfa
Windows Forms aplication for ximc

C# 6-axes interface for XIMC motor controllers

The program allows to operate with up to 6 devices in real time on 3 charts.

To try precompiled example just run ..\testwfa\compiled-win[32/64]\testwfa.exe

To modify the example place all files from win[32/64] folder near the ..\testwfa\testwfa.sln and open this Visual Studio solution. 
The precompiled examples were build with Visual Studio 2013.

Getting started:
Press "search" button to connect with your devices. Then choose X axes and Y axes of each chart using comboboxes in "Chart axes" group box and press "Apply" button. After that you will be able to move your devices.

Simple move:
Click on the chart to move the motors to selected position.

Moving in predefined position:
Choose device to move in "Current device" combobox, input position in "Move to" textbox and press "Go" button for that.

Setting zero:
You can set current position of all devices as zero position by pressing "Set" button in "Set current position as 0" group box.

Charts configuring:
You can remove all points from charts by pressing "Clear" button and enable autoclear mode by checking "Autoclear" checkbox.
When autoclear mode enable first points from the plots will be erasing with time.

"Red button":
Press "Stop all" button to stop all your devices.   

!Attention! The example does not set the correct controller configuration for work with the motors, the apprpriate profile should be set before the launch of the example. The easiest way to set the correct profile is to open the controller via the XiLab software -> open "Settings" menu -> click "Restore from file" -> select the appropriate .cfg file for the used stage/motor.
