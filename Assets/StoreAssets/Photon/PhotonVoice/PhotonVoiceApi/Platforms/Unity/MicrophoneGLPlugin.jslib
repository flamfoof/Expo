var MicrophoneGLPlugin = {

  buffer: undefined,

getMicrophoneDevices: function()
	{
		if(document.microphoneDevices == undefined)
			document.microphoneDevices = new Array();

		if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
			 console.log("enumerateDevices() not supported.");

			 SendMessage('Microphone', 'SetMicrophoneDevices', JSON.stringify(document.microphoneDevices));
			 return;
		}

		navigator.mediaDevices.enumerateDevices()
		 .then(function(devices) {

			var outputDevicesArr = new Array();

			devices.forEach(function(device) {
				if(device.kind == "audioinput"){
					outputDevicesArr.push(device);
				}
			  });

			document.microphoneDevices = outputDevicesArr;

		    SendMessage('Microphone', 'SetMicrophoneDevices', JSON.stringify(document.microphoneDevices));
		 })
		 .catch(function(err) {
			 console.log("get devices exception: " + err.name + ": " + err.message + "; " + err.stack);
		 });
	},
	
	start: function(device, loop, length, frequency){
 
		 document.microphoneFrequency = frequency;

		 function begin(){
			if (navigator.mediaDevices.getUserMedia) {
				navigator.mediaDevices.getUserMedia({
						audio: true
					}).then(GetUserMediaSuccess).catch(GetUserMediaFailed);
			}
		 }
 
		 begin();
		 
		 function GetUserMediaSuccess(stream)
		 {
			 if(document.audioContext == null || document.audioContext == undefined){
			 	document.audioContext = new AudioContext();
			 }
			 document.microphone_stream = document.audioContext.createMediaStreamSource(stream);
			 document.script_processor_node = document.audioContext.createScriptProcessor(0, 1, 1);	
			 document.script_processor_node.onaudioprocess = MicrophoneProcess;
 
			 document.script_processor_node.connect(document.audioContext.destination);
			 document.microphone_stream.connect(document.script_processor_node);
 
			 document.isRecording = 1;
 
			 console.log('record started');		
		 }

		 function GetUserMediaFailed(error)
		 {
			console.log('GetUserMedia failed with error ' + error);	
		 }
	 
		 function MicrophoneProcess(event)
		 {		
			if(IsSafari() || IsEdge()){
				var leftFloat32Array = event.inputBuffer.getChannelData(0);
				var stringArray = "";
 
				for (var i = 0; i < leftFloat32Array.length; i++) {
					stringArray = stringArray + leftFloat32Array[i];
					if(i < leftFloat32Array.length - 1){
						stringArray = stringArray + ",";
					}
				}
	
				SendMessage('Microphone', 'WriteBufferFromMicrophoneHandler', stringArray);
			}
			else {
				Resample(event.inputBuffer);
			}
		 }

		 function Resample(sourceAudioBuffer)
		 {
			var TARGET_SAMPLE_RATE = document.microphoneFrequency;

			var offlineCtx = new OfflineAudioContext(sourceAudioBuffer.numberOfChannels, sourceAudioBuffer.duration * sourceAudioBuffer.numberOfChannels * TARGET_SAMPLE_RATE, TARGET_SAMPLE_RATE);
			var buffer = offlineCtx.createBuffer(sourceAudioBuffer.numberOfChannels, sourceAudioBuffer.length, sourceAudioBuffer.sampleRate);
			// Copy the source data into the offline AudioBuffer
			for (var channel = 0; channel < sourceAudioBuffer.numberOfChannels; channel++) {
				buffer.copyToChannel(sourceAudioBuffer.getChannelData(channel), channel);
			}
			// Play it from the beginning.
			var source = offlineCtx.createBufferSource();
			source.buffer = sourceAudioBuffer;
			source.connect(offlineCtx.destination);
			source.start(0);
			offlineCtx.oncomplete = function(e) {
			  // `resampled` contains an AudioBuffer resampled at 16000Hz.
			  // use resampled.getChannelData(x) to get an Float32Array for channel x.
			  var resampled = e.renderedBuffer;
			  var leftFloat32Array = resampled.getChannelData(0);
			  // use this float32array to send the samples to the server or whatever
			  var stringArray = "";
 
			  for (var i = 0; i < leftFloat32Array.length; i++) {
				  stringArray = stringArray + leftFloat32Array[i];
				  if(i < leftFloat32Array.length - 1){
					  stringArray = stringArray + ",";
				  }
			  }
  
			  SendMessage('Microphone', 'WriteBufferFromMicrophoneHandler', stringArray);
			}
			offlineCtx.startRendering();
		 }

		 function IsSafari(){
			return /constructor/i.test(window.HTMLElement) || (function (p) { return p.toString() === "[object SafariRemoteNotification]"; })(!window['safari'] || (typeof safari !== 'undefined' && safari.pushNotification));
		 }

		 function IsEdge(){
			var isIE = /*@cc_on!@*/false || !!document.documentMode;
			return !isIE && !!window.StyleMedia;
		 }
	},
 
	end: function(device){
		 if(document.microphone_stream != undefined){
			 document.microphone_stream.disconnect(document.script_processor_node);
			 document.script_processor_node.disconnect();
		 }

		 document.microphone_stream = null;
		 document.script_processor_node = null;
 
		 document.isRecording = 0;
 
		 console.log('record ended');	
	},
 
	isRecording: function(device){
		 if(document.isRecording == undefined)
			 document.isRecording = 0;
		 return document.isRecording;
	},
 
	getDeviceCaps: function(device){
		 var returnStr = JSON.stringify(new Array(16000, 44100));
		 var bufferSize = lengthBytesUTF8(returnStr) + 1;
		 var buffer = _malloc(bufferSize);
		 stringToUTF8(returnStr, buffer, bufferSize);
		 return buffer;
	},
 
	isAvailable: function(){
		 return !!(navigator.mediaDevices.getUserMedia);
	},
 
	requestPermission: function(){

		if (navigator.mediaDevices.getUserMedia) {
			navigator.mediaDevices.getUserMedia({ audio: true }).then();

			function GetUserMediaSuccess(stream){
			 	SendMessage('Microphone', 'PermissionUpdate', "granted");
			}
		}
	},
 
	hasUserAuthorizedPermission: function(){
		try{
			navigator.permissions.query(
				{ name: 'microphone' }
			).then(function(permissionStatus){	
				SendMessage('Microphone', 'PermissionUpdate', permissionStatus.state.toString());
			});
		}
		catch(err){
			console.log("hasUserAuthorizedPermission exception: " + err.name + ": " + err.message + "; " + err.stack);
		}
	},
	
  Init: function() {

    console.log("Init:");
  
	// START - used to read the volume
	document.volume = 0;
	var byteOffset = 0;
	var length = 1024;
	this.buffer = new ArrayBuffer(4 * length);
    document.dataArray = new Float32Array(this.buffer, byteOffset, length);
	// END - used to read the volume

	navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;

	if (navigator.getUserMedia) {
  
		var constraints = {
		  audio: {
			optional: [{
			  sourceId: "audioSource"
			}]
		  }
		};
		navigator.getUserMedia(constraints, function(stream) {
		  console.log('navigator.getUserMedia successCallback: ', stream);
	  
		  document.position = 0;

		  document.audioContext = new AudioContext();
		  document.tempSize = 1024;
		  document.tempArray = new Float32Array(document.tempSize)
		  document.analyser = document.audioContext.createAnalyser();
		  document.analyser.minDecibels = -90;
		  document.analyser.maxDecibels = -10;
		  document.analyser.smoothingTimeConstant = 0.85;

		  document.mediaRecorder = new MediaRecorder(stream);

		  document.source = document.audioContext.createMediaStreamSource(stream);

		  document.source.connect(document.analyser);

		  document.mediaRecorder.start();
		  console.log(document.mediaRecorder.state);

		  document.readDataOnInterval = function() {

			if (document.dataArray == undefined) {
			  setTimeout(document.readDataOnInterval, 250); //wait to be set
			  return;
			}

			document.tempInterval = Math.floor(document.tempSize / document.dataArray.length * 250);

			// read the next chunk after interval
			setTimeout(document.readDataOnInterval, document.tempInterval); //if mic is still active

			if (document.dataArray == undefined) {
			  return;
			}

			//read the temp data buffer
			document.analyser.getFloatTimeDomainData(document.tempArray);

			// use the amplitude to get volume
            document.volume = 0;

			var j = (document.position + document.dataArray.length - document.tempSize) % document.dataArray.length;
			for (var i = 0; i < document.tempSize; ++i) {
			  document.volume = Math.max(document.volume, Math.abs(document.tempArray[i]));
			  document.dataArray[j] = document.tempArray[i];
			  j = (j + 1) % document.dataArray.length;
			}
			document.position = (document.position + document.tempSize) % document.dataArray.length;

		  };

		  document.readDataOnInterval();


		}, function(error) {
		  console.error('navigator.getUserMedia errorCallback: ', error);
		});
	}
  },
  

  
  QueryAudioInput: function() {

    console.log("QueryAudioInput");

    document.mMicrophones = [];

    if (!navigator.mediaDevices || !navigator.mediaDevices.enumerateDevices) {
      console.log("enumerateDevices() not supported.");
    } else {
      // List microphones
      navigator.mediaDevices.enumerateDevices()
      .then(function(devices) {
        devices.forEach(function(device) {
          console.log("QueryAudioInput: kind="+device.kind + " device=", device, " label=" + device.label);
          if (device.kind === 'audioinput') {
            document.mMicrophones.push(device.label);
          }
        });
      })
      .catch(function(err) {
        console.error(err.name + ": " + err.message);
      });
    }
  },
  
  GetNumberOfMicrophones: function() {
    console.log("GetNumberOfMicrophones");
	var microphones = document.mMicrophones;
    if (microphones == undefined) {
	  console.log("GetNumberOfMicrophones", 0);
      return 0;
    }  
    console.log("GetNumberOfMicrophones length="+microphones.length);
    return microphones.length;
  },
  
  GetMicrophoneDeviceName: function(index) {
	//console.log("GetMicrophoneDeviceName");
	var returnStr = "Not Set";
	var microphones = document.mMicrophones;
    if (microphones != undefined) {
      if (index >= 0 && index < microphones.length) {
	    if (microphones[index] != undefined) {
		  returnStr = microphones[index];
		}
      }
    }  
	console.log("GetMicrophoneDeviceName", returnStr);
    var buffer = _malloc(lengthBytesUTF8(returnStr) + 1);
    writeStringToMemory(returnStr, buffer);
    return buffer;
  },

  GetMicrophoneVolume: function(index) {
	console.log("GetMicrophoneVolume");
    if (document.volume == undefined) {
	   return 0;
	}
	console.log("GetMicrophoneVolume", document.volume);
    return document.volume;
  }
};

mergeInto(LibraryManager.library, MicrophoneGLPlugin);
