# The Unity-BCI API
The Unity-BCI API allows game developers to use the data from brain-computer interfaces, such as EEG or HEG systems, directly in their games.

Setting up the asset is as simple as adding the asset folder, creating a controller, and adding the device types you want to use. Data can be accessed elegantly and easily, and can be used directly or averaged.

### Quick Start:
* Add the unity_bci_api folder to your Assets folder.
* Create an empty, or use an existing empty and add the MainController script to it.
* Create a reference to the script inside your main code.
* Use the mainController.Add() method to add sensors: ```mainController.Add(new HEGduino(new SerialConnection("/dev/tty.usbserial-01DFAAE3"))```
    * *You can initialize both the connection and the device controller seperately and then add them, here we do it inline.*
* Run mainController.Start()
* Access the data: ```Print(mainController.brain_bloodflow)``` / ```x = mainController.heartrate.WeightedAverage(0.1)```

### Sensors:
* HEGduino
* EmotiBit (WIP)
* CytonEEG (WIP)

### Data:
* brain_bloodflow (HEGduino)
* heartrate (EmotiBit) (WIP)
* ... more being added.
