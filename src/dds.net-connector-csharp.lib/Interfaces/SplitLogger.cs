namespace DDS.Net.Connector.Interfaces
{
    public class SplitLogger : ILogger
    {
        private readonly ILogger logger01;
        private readonly ILogger logger02;
        private readonly ILogger logger03;
        private readonly ILogger logger04;
        private readonly ILogger logger05;

        public SplitLogger(
            ILogger logger01,
            ILogger logger02,
            ILogger logger03 = null!,
            ILogger logger04 = null!,
            ILogger logger05 = null!)
        {
            this.logger01 = logger01 ?? throw new ArgumentNullException(nameof(logger01));
            this.logger02 = logger02 ?? throw new ArgumentNullException(nameof(logger02));
            this.logger03 = logger03;
            this.logger04 = logger04;
            this.logger05 = logger05;
        }

        public void Error(string message)
        {
            logger01?.Error(message);
            logger02?.Error(message);
            logger03?.Error(message);
            logger04?.Error(message);
            logger05?.Error(message);
        }

        public void Info(string message)
        {
            logger01?.Info(message);
            logger02?.Info(message);
            logger03?.Info(message);
            logger04?.Info(message);
            logger05?.Info(message);
        }

        public void Warning(string message)
        {
            logger01?.Warning(message);
            logger02?.Warning(message);
            logger03?.Warning(message);
            logger04?.Warning(message);
            logger05?.Warning(message);
        }
    }
}
