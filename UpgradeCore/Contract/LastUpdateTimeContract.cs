namespace UpgradeCore
{
    public class LatestUpdateTimeContract
    {
        public int ClientVersion { get; set; } = 0;

        public LatestUpdateTimeContract() {}

        public LatestUpdateTimeContract(int version)
        {           
            ClientVersion = version;
        }
    }
}
