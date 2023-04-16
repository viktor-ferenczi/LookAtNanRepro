# MyTurretControlBlock.LookAt returns NaN 

## Summary

This is a .NET Framework 4.8 Console Application to reproduce the root cause of a [crash bug](https://support.keenswh.com/spaceengineers/pc/topic/27973-arithmeticexception-function-does-not-accept-floating-point-not-a-number-values) in Space Engineers version 1.202.066.

## Root Cause Analysis

Found the root cause of the problem in the `MyTurretControlBlock.LookAt` method by code review.

`NaN` results are produced in the `X` or `Y` components of the resulting `Vector3` returned
from `MyTurretControlBlock.LookAt` when the `target` is parallel or nearly parallel to the
`Up` vector of the Elevator or the Azimuthor rotors respectively, but the `ShootDirection` 
is not parallel.

## Test case decoupled from the game 

Please edit and run the `Edit-and-run-before-opening-solution.bat` script before opening the 
solution. It will synlink the game's `Bin64` folder, so the DLL dependencies can be found.

The calculation from the decompiled `LookAt` method has been copied into this console application
to find actual examples when the calculation fails and produces `NaN` results. The test varies the
`ShootDirection` from `Up` to slightly rotated around the `Right` axis. 
This simple test fails in 8 out of 10 cases.

The absolute value of the dot product used in your guarding conditions varies between 
`0.9998` and `0.99995`. Even when the absolute value is less than `0.9999` the calculation
can fail and produce `NaN` values on the following line:

```csharp
Vector3 vector1 = Vector3.Normalize(vector2_1 - up * Vector3.Dot(up, vector2_1));
```

Inside the `Normalize` method there is a division by zero, resulting in an `Infinity`. 
Multiplying `X`, `Y` and `Z` by `Infinity` results in a vector full of `NaN` values.

This may happen both in the azimuthor and the elevator calculations.

## Test output

```
OK: ShootDirection=(X:0 Y:1 Z:0) => {X:0 Y:0 Z:0}
OK: ShootDirection=(X:0 Y:0.999950000416665 Z:0.00999983333416666) => {X:0 Y:0 Z:0}
FAIL: ShootDirection=(X:0 Y:0.999800006666578 Z:0.0199986666933331) => {X:NaN Y:NaN Z:0}
FAIL: ShootDirection=(X:0 Y:0.999550033748988 Z:0.0299955002024957) => {X:NaN Y:NaN Z:0}
FAIL: ShootDirection=(X:0 Y:0.999200106660978 Z:0.0399893341866342) => {X:NaN Y:NaN Z:0}
FAIL: ShootDirection=(X:0 Y:0.998750260394966 Z:0.0499791692706783) => {X:NaN Y:NaN Z:0}
FAIL: ShootDirection=(X:0 Y:0.998200539935204 Z:0.0599640064794446) => {X:NaN Y:NaN Z:0}
FAIL: ShootDirection=(X:0 Y:0.99755100025328 Z:0.0699428473375328) => {X:NaN Y:NaN Z:0}
FAIL: ShootDirection=(X:0 Y:0.996801706302619 Z:0.0799146939691727) => {X:NaN Y:NaN Z:0}
FAIL: ShootDirection=(X:0 Y:0.995952733011994 Z:0.089878549198011) => {X:NaN Y:NaN Z:0}
Total: 10
Failed: 8
LowestAzimuthorAbsDotProduct = 0.999949991703033
HighestAzimuthorAbsDotProduct = 0.999800026416779
LowestElevatorAbsDotProduct = 0.999949991703033
HighestElevatorAbsDotProduct = 0.999800026416779
```