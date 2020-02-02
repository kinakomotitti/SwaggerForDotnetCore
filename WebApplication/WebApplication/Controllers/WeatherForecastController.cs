using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private IFirstService _firstService;
        private IThirdService _thirdService;
        private IFifthService _fifthService;
        private ISeventhService _seventhService;
        private ISecondService _secondService;
        private IServiceScopeFactory _serviceScopeFactory;
        public WeatherForecastController(ILogger<WeatherForecastController> logger,
                                         IFirstService firstService,
                                         ISecondService secondService,
                                         IThirdService thirdService,
                                         IFifthService fifthService,
                                         ISeventhService seventhService,
                                         IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            this._firstService = firstService;
            this._thirdService = thirdService;
            this._fifthService = fifthService;
            this._secondService = secondService;
            this._seventhService = seventhService;
            this._serviceScopeFactory = serviceScopeFactory;
        }

        [HttpGet]
        public IActionResult Get()
        {
            //try { this._fifthService.Execute(); } catch (Exception ex) { Console.WriteLine(ex.GetBaseException().Message); }
            //try { this._thirdService.Execute(); } catch (Exception ex) { Console.WriteLine(ex.GetBaseException().Message); }
            //try { this._firstService.Execute(); } catch (Exception ex) { Console.WriteLine(ex.GetBaseException().Message); }
            //try { this._seventhService.Execute(); } catch (Exception ex) { Console.WriteLine(ex.GetBaseException().Message); }


            Task.Run(() =>
            {
                using (var scope = this._serviceScopeFactory.CreateScope())
                {
                    var newService = scope.ServiceProvider.GetService<ISecondService>();
                    newService.Execute();
                }
            });

            return Ok();
        }
    }

    #region Normal

    public interface IFirstService { void Execute(); }
    public class FirstService : IFirstService
    {
        private dotnetContext _dotnetContext;
        private ISecondService _secondService;
        public FirstService(dotnetContext dotnetContext, ISecondService secondService)
        {
            this._dotnetContext = dotnetContext;
            this._secondService = secondService;
        }

        public void Execute()
        {
            Task.Delay(3000);
            this._secondService.Execute();
            Task.Delay(3000);
            this._dotnetContext.TransactTest.Add(new TransactTest()
            {
                Id = int.Parse(DateTime.Now.ToString("hhmmss")),
                Text = "First Insert"
            }); ;
            this._dotnetContext.SaveChanges();
            this._secondService.Execute();
        }
    }

    public interface ISecondService { void Execute(); }
    public class SecondService : ISecondService
    {
        private dotnetContext _dotnetContext;
        public SecondService(dotnetContext dotnetContext) { this._dotnetContext = dotnetContext; }

        public void Execute()
        {
            try
            {
                this._dotnetContext.TransactTest.Add(new TransactTest()
                {
                    Id = int.Parse(DateTime.Now.ToString("hhmmss")),
                    Text = "Second Insert"
                });
                this._dotnetContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    #endregion

    #region WithTransaction


    public interface IThirdService { void Execute(); }
    public class ThirdService : IThirdService
    {
        private dotnetContext _dotnetContext;
        private IFourthService _fourthService;
        public ThirdService(dotnetContext dotnetContext, IFourthService fourthService)
        {
            this._dotnetContext = dotnetContext;
            this._fourthService = fourthService;
        }

        public void Execute()
        {
            using (var transaction = this._dotnetContext.Database.BeginTransaction())
            {
                this._fourthService.Execute();
                this._dotnetContext.TransactTest.Add(new TransactTest()
                {
                    Id = int.Parse(DateTime.Now.ToString("hhmmss")),
                    Text = "Third Insert"
                });
                this._dotnetContext.SaveChanges();
                transaction.Rollback();
                this._fourthService.Execute();
            }
        }
    }
    public interface IFourthService { void Execute(); }
    public class FourthService : IFourthService
    {
        private dotnetContext _dotnetContext;
        public FourthService(dotnetContext dotnetContext) { this._dotnetContext = dotnetContext; }

        public void Execute()
        {
            using (var transaction = this._dotnetContext.Database.BeginTransaction()) //既にTransactionが開始されているため、エラー発生
            {
                this._dotnetContext.TransactTest.Add(new TransactTest()
                {
                    Id = int.Parse(DateTime.Now.ToString("hhmmss")),
                    Text = "Fourth Insert"
                });
                this._dotnetContext.SaveChanges();
            }
        }
    }

    #endregion

    #region Parent(Fifth) with transaction

    public interface IFifthService { void Execute(); }
    public class FifthService : IFifthService
    {
        private dotnetContext _dotnetContext;
        private ISixthService _sixthService;
        public FifthService(dotnetContext dotnetContext, ISixthService sixthService)
        {
            this._dotnetContext = dotnetContext;
            this._sixthService = sixthService;
        }

        public void Execute()
        {
            using (var transaction = this._dotnetContext.Database.BeginTransaction())
            {
                this._sixthService.Execute();
                this._dotnetContext.TransactTest.Add(new TransactTest()
                {
                    Id = int.Parse(DateTime.Now.ToString("hhmmss")),
                    Text = "Fifth Insert"
                });
                this._dotnetContext.SaveChanges();
                transaction.Rollback();
                this._sixthService.Execute();
            }
        }
    }
    public interface ISixthService { void Execute(); }
    public class SixthService : ISixthService
    {
        private dotnetContext _dotnetContext;
        public SixthService(dotnetContext dotnetContext) { this._dotnetContext = dotnetContext; }

        public void Execute()
        {
            this._dotnetContext.TransactTest.Add(new TransactTest()
            {
                Id = int.Parse(DateTime.Now.ToString("hhmmss")),
                Text = "Sixth Insert"
            });
            this._dotnetContext.SaveChanges();//Transactionがロールバックされた後は、実行される
        }
    }

    #endregion

    #region Child(Seventh) with transaction

    public interface ISeventhService { void Execute(); }
    public class SeventhService : ISeventhService
    {
        private dotnetContext _dotnetContext;
        private IEighthService _eighthService;
        public SeventhService(dotnetContext dotnetContext, IEighthService eighthService)
        {
            this._dotnetContext = dotnetContext;
            this._eighthService = eighthService;
        }

        public void Execute()
        {
            this._eighthService.Execute();
            this._dotnetContext.TransactTest.Add(new TransactTest()
            {
                Id = int.Parse(DateTime.Now.ToString("hhmmss")),
                Text = "Seventh Insert"
            });
            this._dotnetContext.SaveChanges();
            this._eighthService.Execute();
        }
    }
    public interface IEighthService { void Execute(); }
    public class EighthService : IEighthService
    {
        private dotnetContext _dotnetContext;
        public EighthService(dotnetContext dotnetContext) { this._dotnetContext = dotnetContext; }

        public void Execute()
        {
            using (var transaction = this._dotnetContext.Database.BeginTransaction())
            {
                this._dotnetContext.TransactTest.Add(new TransactTest()
                {
                    Id = int.Parse(DateTime.Now.ToString("hhmmss")),
                    Text = "Eighth Insert"
                });
                this._dotnetContext.SaveChanges();
                transaction.Rollback();
            }
        }
    }

    #endregion
}