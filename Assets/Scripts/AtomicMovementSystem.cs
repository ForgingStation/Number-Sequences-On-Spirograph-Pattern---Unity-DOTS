using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class AtomicMovementSystem : SystemBase
{
    private static bool Initialized = false;
    private float changeEvery = 0;
    private float ratioqn = 5;
    private int largeRadius = 30;
    private int smallRadius = 20;
    private float ratioqd = 3;
    private bool isNegetive;

    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        if (!Initialized)
        {
            largeRadius = Pattern_Generator.instance.largeRadius;
            smallRadius = Pattern_Generator.instance.smallRadius;
            ratioqd = Pattern_Generator.instance.initialRatioQd;
            ratioqn = Pattern_Generator.instance.initialRatioQn;
            changeEvery = Pattern_Generator.instance.changeEvery;
            Initialized = true;
        }
        changeEvery -= Time.DeltaTime;

        if (changeEvery <= 0 && Initialized)
        {
            //ratioqn = ratioqn + 1; //Natural - 1
            ratioqn = (ratioqn - 1) + (ratioqn - 2); // Fib-5
            //ratioqn = (ratioqn - 2) + (ratioqn - 3); //Padovan-5
            //ratioqn = (ratioqn - 1) + ratioqn; //Recamán's sequence-1
            //ratioqn = (2*(ratioqn - 1)) + (ratioqn - 2); //Pell-3

            ratioqd = (ratioqd - 1) +(ratioqd - 2);//Fib-4
            //ratioqd = (ratioqd - 2) + (ratioqd - 3); //Padovan-2
            //ratioqd = (ratioqd - 1) + ratioqd;//Recamán's sequence-2
            //ratioqd = (2 * (ratioqd - 1)) + (ratioqd - 2);//Pell-2

            float ratioQnForJob = ratioqn;
            int largeRadiusForJob = largeRadius;
            int smallRadiusForJob = smallRadius;
            float ratioqdForJob = ratioqd;
            if (!isNegetive)
            {
                ratioQnForJob = ratioQnForJob * -1;
                isNegetive = true;
            }
            else
            {
                isNegetive = false;
            }
            Entities.WithBurst().ForEach((ref AtomicUnit au) =>
            {
                float xValue = (largeRadiusForJob * math.cos(au.entityId)) +
                                (smallRadiusForJob * (math.cos(((ratioQnForJob) / ratioqdForJob) * au.entityId)));

                float zValue = (largeRadiusForJob * math.sin(au.entityId)) +
                                (smallRadiusForJob * (math.sin(((ratioQnForJob) / ratioqdForJob) * au.entityId)));
                au.toLocation = new float3(xValue, 0, zValue);
                au.reached = false;
                au.ratioQnDebug = ratioQnForJob;
             }).ScheduleParallel();
            changeEvery = Pattern_Generator.instance.changeEvery;
        }

        Entities.WithBurst().ForEach((ref AtomicUnit au, ref Translation trans) =>
        {
            if (!au.reached)
            {
                au.waypointDirection = math.normalize(au.toLocation - trans.Value);
                trans.Value += au.waypointDirection * au.Speed * deltaTime;

                if (math.distance(trans.Value, au.toLocation) <= au.minDistanceReached)
                {
                    au.reached = true;
                }
            }
        }).ScheduleParallel();
    }

}
