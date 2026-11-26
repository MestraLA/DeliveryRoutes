namespace DeliveryRoutes.Backend
{
    internal enum StopKind
    {
        Pickup,
        Delivery
    }

    internal readonly record struct Stop(
        string Location,
        StopKind Kind,
        int PackageId);
}