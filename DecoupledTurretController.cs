using System;
using VRageMath;

namespace LookAtNanRepro
{
    public class DecoupledTurretController
    {
        public Vector3D ShootDirection = Vector3D.Forward;
        private Vector3D ShootOrigin = Vector3D.Zero;

        private Vector3D AzimuthorUp = Vector3D.Up;
        private float AzimuthorAngle = 0f;

        private Vector3D ElevatorUp = Vector3D.Up;
        private float ElevatorAngle = 0f;
        
        public double LowestAzimuthorAbsDotProduct = 1;
        public double HighestAzimuthorAbsDotProduct = 0;
        
        public double LowestElevatorAbsDotProduct = 1;
        public double HighestElevatorAbsDotProduct = 0;

        // Decoupled static version of the MyTurretControlBlock.LookAt method.
        // It contains the same relevant arithmetic, but does not depend on actual block instances.
        // Abs dot products are extracted into variables and min/max kept for diagnostics
        // helping to narrow down the 0.9999 limit.
        // Game version 1.202.066, decompiled by JetBrains Rider
        public Vector3 LookAt(Vector3D target)
        {
            Vector3 shootDirection = (Vector3)this.ShootDirection;
            Vector3D shootOrigin = this.ShootOrigin;
            // if (object.Equals((object) shootOrigin, (object) this.Target))
            //   return new Vector3(this.Elevator != null ? this.Elevator.GetAngle() : 0.0f, this.Azimuthor != null ? this.Azimuthor.GetAngle() : 0.0f, 0.0f);
            Vector3 vector2_1 = (Vector3)Vector3D.Normalize(target - shootOrigin);
            Vector3 vector3 = new Vector3(0.0f, 0.0f, 0.0f);
            if (true) // if (this.Azimuthor != null)
            {
                // double rotorAngularVelocity = (double) this.Azimuthor.MaxRotorAngularVelocity;
                Vector3 up = (Vector3)AzimuthorUp; // this.Azimuthor.PositionComp.WorldMatrix.Up;
                // this.Azimuthor.PositionComp.GetPosition();
                var absDotProduct = Math.Abs(Vector3D.Dot((Vector3D)shootDirection, up));
                if (absDotProduct > 0.9999)
                {
                    LowestAzimuthorAbsDotProduct = Math.Min(LowestAzimuthorAbsDotProduct, absDotProduct);
                    // this.Azimuthor.TargetVelocity.Value = 0.0f;
                }
                else
                {
                    HighestAzimuthorAbsDotProduct = Math.Max(HighestAzimuthorAbsDotProduct, absDotProduct);
                    Vector3 vector2_2 = Vector3.Normalize(shootDirection - up * Vector3.Dot(up, shootDirection));
                    Vector3 vector1 = Vector3.Normalize(vector2_1 - up * Vector3.Dot(up, vector2_1));
                    float num = ((double)Vector3.Dot(vector1, (Vector3)Vector3D.Cross((Vector3D)up, (Vector3D)vector2_2)) <= 0.0 ? 1f : -1f) * (float)Math.Acos((double)MyMath.Clamp(Vector3.Dot(vector1, vector2_2), -1f, 1f));
                    float angle = AzimuthorAngle; // this.Azimuthor.GetAngle();
                    vector3.Y = MathHelper.WrapAngle(angle + num);
                }
            }

            if (true) // if (this.Elevator != null)
            {
                // double rotorAngularVelocity = (double) this.Elevator.MaxRotorAngularVelocity;
                Vector3 up = (Vector3)ElevatorUp; //  this.Elevator.PositionComp.WorldMatrix.Up;
                // this.Elevator.PositionComp.GetPosition();
                var absDotProduct = Math.Abs(Vector3D.Dot((Vector3D)shootDirection, up));
                if (absDotProduct <= 0.9999)
                {
                    HighestElevatorAbsDotProduct = Math.Max(HighestElevatorAbsDotProduct, absDotProduct);
                    Vector3 vector2_3 = Vector3.Normalize(shootDirection - up * Vector3.Dot(up, shootDirection));
                    Vector3 vector1 = Vector3.Normalize(vector2_1 - up * Vector3.Dot(up, vector2_1));
                    float num = ((double)Vector3.Dot(vector1, (Vector3)Vector3D.Cross((Vector3D)up, (Vector3D)vector2_3)) <= 0.0 ? 1f : -1f) * (float)Math.Acos((double)MyMath.Clamp(Vector3.Dot(vector1, vector2_3), -1f, 1f));
                    float angle = ElevatorAngle; // this.Elevator.GetAngle();
                    vector3.X = MathHelper.WrapAngle(angle + num);
                }
                else
                {
                    LowestElevatorAbsDotProduct = Math.Min(LowestElevatorAbsDotProduct, absDotProduct);
                }
            }

            return vector3;
        }
    }
}