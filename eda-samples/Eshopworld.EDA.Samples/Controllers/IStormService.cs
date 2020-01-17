using Eshopworld.EDA.Samples.Models;

namespace Eshopworld.EDA.Samples.Controllers
{
    public interface IStormService
    {
        void DoBusinessLogic(StormModel stormModel);
    }
}