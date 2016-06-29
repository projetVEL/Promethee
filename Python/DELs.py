import Constellation
import RPi.GPIO as gpio
import time

gpio.setmode(gpio.BCM)
gpio.setupwarnings(False)
defaultPin=26 # celle au dessus du GND en bas à gauche
dictionaryON = {}
inBoucle = False;

@Constellation.MessageCallBack()
def definirPinDefaut(pin):
    defaultPin = pin;
    gpio.setup(defaultPin,gpio.OUT)

@Constellation.MessageCallback()
def ChangerEtatDELs(pin):
    if(dictionaryON.get(pin)==None):
        gpio.setup(defaultPin,gpio.OUT)
        dictionaryON[pin]=False;
    inBoucle = False;
    "allume ou éteind les DELs"
    if(dictionaryON[pin]):
        dictionaryON[pin] = False
        gpio.output(pin,gpio.LOW)
    else:
        dictionaryON[pin] = True
        gpio.output(pin,gpio.HIGH)

@Constellation.MessageCallback()
def AllumerDELs(pin):
    if(dictionaryON.get(pin)==None):
        gpio.setup(defaultPin,gpio.OUT)
    inBoucle = False;
    "allume les DELs"
    dictionaryON[pin]=True;
    gpio.output(pin,gpio.HIGH)

@Constellation.MessageCallback()
def EteindreDELs(pin):
    if(dictionaryON.get(pin)==None):
        gpio.setup(defaultPin,gpio.OUT)
    inBoucle = False
    "allume les DELs"
    dictionaryON[pin]=False
    gpio.output(pin,gpio.LOW)

@Constellation.MessageCallback()
def Clignoter(pin, dureeTotale, dureeHaute, dureeBasse):
    if(dictionaryON.get(pin)==None):
        gpio.setup(defaultPin,gpio.OUT)
        dictionaryON[pin]=False
    inBoucle = True
    wait = 0
    while(inBoucle):
        if(dictionaryON[pin]):
            gpio.output(pin,gpio.HIGH)
            wait = dureeHaute
        else:
            gpio.output(pin,gpio.LOW)
            wait = dureeBasse
    dictionaryON[pin] = not dictionaryON[pin]
    time.sleep(wait)
    dureeTotale -= wait
    if(dureeTotale <= 0):
        inBoucle = False
        
def OnExit():
    gpio.output(26,gpio.LOW)
    pass

def OnStart():
    # Register callback on package shutdown
    Constellation.OnExitCallback = OnExit   
    # Write log & common properties
    Constellation.WriteInfo("Hi I'm '%s' and I run on %s. IsConnected = %s | IsStandAlone = %s " % (Constellation.PackageName, Constellation.SentinelName, Constellation.IsConnected, Constellation.IsStandAlone))
    
Constellation.Start(OnStart);