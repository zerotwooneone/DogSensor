#include <SoftTimer.h>
#include <DelayRun.h>

const int LedPin = 13;
const int buzzerPin = 3;//the buzzer pin attach to
const int trigPin = 11;    // Trigger
const int echoPin = 12;    // Echo

// -- Define method signatures.
boolean turnOff(Task* task);
boolean turnOn(Task* task);
boolean turnOnTrig(Task* task);
boolean turnOffTrig(Task* task);

const int trigCleanupMilliseconds = 200;
const int trigTaskMilliseconds = 10;
DelayRun trigOffTask(trigTaskMilliseconds, turnOffTrig);
DelayRun trigOnTask(trigCleanupMilliseconds, turnOnTrig, &trigOffTask);

void setup() {
  //Serial Port begin
  Serial.begin (115200);
  
  // -- We close the loop, so after offTask the onTask will start.
  trigOffTask.followedBy = &trigOnTask;

  pinMode(LedPin, OUTPUT);
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  pinMode(buzzerPin,OUTPUT);

  // -- Start the offTask to take effect after 1 second.
  trigOnTask.startDelayed();
}

boolean turnOnTrig(Task* task) {
  digitalWrite(trigPin, HIGH);
  return true; // -- Return true to enable the "followedBy" task.
}
boolean turnOffTrig(Task* task) {
  digitalWrite(trigPin, LOW);

  // Read the signal from the sensor: a HIGH pulse whose
  // duration is the time (in microseconds) from the sending
  // of the ping to the reception of its echo off of an object.
  long duration;
  duration = pulseIn(echoPin, HIGH);
 
  // Convert the time into a distance
  long cm;
  cm = (duration/2) / 29.1;     // Divide by 29.1 or multiply by 0.0343

  if(cm < 91){
    digitalWrite(LedPin, HIGH);
    tone(buzzerPin,cm); //turn the buzzer on
  }else{
    digitalWrite(LedPin, LOW);
    noTone(buzzerPin);
  }

  Serial.print(cm);
  Serial.print("cm");
  Serial.println();
  
  return true; // -- Return true to enable the "followedBy" task.
}
