//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Entities;
//using Unity.Transforms;
//using Unity.Mathematics;


//public struct NeedRotate : IComponentData
//{
//    public float FinalAngle;
//    public float CurrentAngle;
//    public float Speed;
//}
//public class RotationSystem : ComponentSystem
//{
//    private float deltaTime;

//    protected override void OnUpdate()
//    {
//        deltaTime = Time.DeltaTime;

//        Entities.ForEach((Entity entity, ref Rotation rotation, ref NeedRotate needRotate) =>
//        {
//            // Если угол отрицательный, угловой множитель тоже должен быть отрицательным
//            var angleMult = (needRotate.FinalAngle / math.abs(needRotate.FinalAngle));
//            var angle = angleMult * needRotate.Speed * deltaTime * math.PI / 180;
//            quaternion newRotation = math.mul(rotation.Value, quaternion.RotateY(angle));

//            if (math.abs(needRotate.FinalAngle-needRotate.CurrentAngle)>=3)
//            {
//                needRotate.CurrentAngle += angleMult * needRotate.Speed * deltaTime;
//                PostUpdateCommands.SetComponent(entity, new Rotation { Value = newRotation });
//                PostUpdateCommands.SetComponent(entity, new NeedRotate
//                {
//                    FinalAngle = needRotate.FinalAngle,
//                    CurrentAngle = needRotate.CurrentAngle,
//                    Speed = needRotate.Speed
//                });
//                Debug.Log(math.abs(needRotate.FinalAngle - needRotate.CurrentAngle));
//            }
//            else
//            {
//                PostUpdateCommands.RemoveComponent(entity, typeof(NeedRotate));
//            }
//        });
//    }
//}
