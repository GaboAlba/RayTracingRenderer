namespace RayTracingRenderer.BoundingVolumeHierarchy
{
    public class BVHNode
    {
        //public IHittableObject Left { get; private set; }
        //public IHittableObject Right { get; private set; }
        //public BoundingBox Box { get; private set; }

        //public BVHNode(List<IHittableObject> objects, int start, int end)
        //{
        //    int axis = new Random().Next(0, 3);
        //    int objectSpan = end - start;

        //    if (objectSpan == 1)
        //    {
        //        Left = Right = objects[start];
        //        Box = Left.BoundingBox();
        //    }
        //    else if (objectSpan == 2)
        //    {
        //        if (Compare(objects[start], objects[start + 1], axis) < 0)
        //        {
        //            Left = objects[start];
        //            Right = objects[start + 1];
        //        }
        //        else
        //        {
        //            Left = objects[start + 1];
        //            Right = objects[start];
        //        }
        //        Box = SurroundingBox(Left.BoundingBox(), Right.BoundingBox());
        //    }
        //    else
        //    {
        //        objects.Sort((a, b) => Compare(a, b, axis));
        //        int mid = start + objectSpan / 2;
        //        Left = new BVHNode(objects, start, mid);
        //        Right = new BVHNode(objects, mid, end);
        //        Box = SurroundingBox(Left.BoundingBox(), Right.BoundingBox());
        //    }
        //}

        //public bool HitObject(Ray ray, Interval interval, HitRecord record, out HitRecord outputRecord)
        //{
        //    outputRecord = null;
        //    if (!Box.Hit(ray, interval.MinValue, interval.MaxValue)) return false;

        //    bool hitLeft = Left.HitObject(ray, interval, record, out var leftRecord);
        //    bool hitRight = Right.HitObject(ray, interval, record, out var rightRecord);

        //    if (hitLeft && hitRight)
        //    {
        //        outputRecord = leftRecord.HitTime < rightRecord.HitTime ? leftRecord : rightRecord;
        //        return true;
        //    }
        //    else if (hitLeft)
        //    {
        //        outputRecord = leftRecord;
        //        return true;
        //    }
        //    else if (hitRight)
        //    {
        //        outputRecord = rightRecord;
        //        return true;
        //    }

        //    return false;
        //}

        //public BoundingBox BoundingBox()
        //{
        //    return this.Box;
        //}

        //private static int Compare(IHittableObject a, IHittableObject b, int axis)
        //{
        //    return a.BoundingBox().Min[axis].CompareTo(b.BoundingBox().Min[axis]);
        //}

        //private static BoundingBox SurroundingBox(BoundingBox box0, BoundingBox box1)
        //{
        //    Vector3 small = new Vector3(
        //        Math.Min(box0.Min.X, box1.Min.X),
        //        Math.Min(box0.Min.Y, box1.Min.Y),
        //        Math.Min(box0.Min.Z, box1.Min.Z)
        //    );
        //    Vector3 big = new Vector3(
        //        Math.Max(box0.Max.X, box1.Max.X),
        //        Math.Max(box0.Max.Y, box1.Max.Y),
        //        Math.Max(box0.Max.Z, box1.Max.Z)
        //    );
        //    return new BoundingBox(small, big);
        //}
    }
}
