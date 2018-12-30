/*
 * created by Rui Santos, https://randomnerdtutorials.com
 * 
 * Complete Guide for Ultrasonic Sensor HC-SR04
 *
    Ultrasonic sensor Pins:
        VCC: +5VDC
        Trig : Trigger (INPUT) - Pin11
        Echo: Echo (OUTPUT) - Pin 12
        GND: GND
 */
 
int trigPin = 11;    // Trigger
int echoPin = 12;    // Echo


const int buzzerPin = 3;//the buzzer pin attach to
int fre;//set the variable to store the frequence value

const int minimumCentimeters =91;
 
void setup() {
  //Serial Port begin
  Serial.begin (9600);
  //Define inputs and outputs
  pinMode(trigPin, OUTPUT);
  pinMode(echoPin, INPUT);
  pinMode(buzzerPin,OUTPUT);
}
 
void loop() {
  long cm= getCentimeters();
  
  if(cm < minimumCentimeters)
  {
    for(int i = 200;i <= 800;i++) //frequence loop from 200 to 800
    {
      if(cm < minimumCentimeters){
        tone(buzzerPin,i); //turn the buzzer on    
      }else{
        noTone(buzzerPin);      
        break;
      }
      cm= getCentimeters();
    }
  }
   noTone(buzzerPin);
}

/*
 * This takes at least 15 ms to run 
 */
long getCentimeters(){
  // The sensor is triggered by a HIGH pulse of 10 or more microseconds.
  // Give a short LOW pulse beforehand to ensure a clean HIGH pulse:
  digitalWrite(trigPin, LOW);
  delayMicroseconds(5);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);
 
  // Read the signal from the sensor: a HIGH pulse whose
  // duration is the time (in microseconds) from the sending
  // of the ping to the reception of its echo off of an object.
  pinMode(echoPin, INPUT);
  long duration;
  duration = pulseIn(echoPin, HIGH);
 
  // Convert the time into a distance
  long cm;
  cm = (duration/2) / 29.1;     // Divide by 29.1 or multiply by 0.0343
  
  Serial.print(cm);
  Serial.print("cm");
  Serial.println();
  return cm;
}
