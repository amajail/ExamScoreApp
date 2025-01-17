using ExamScoreApp.Core.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ExamScoreApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamScoreController : ControllerBase
    {
        private IExamScoreService _examScoreService;

        public ExamScoreController(IExamScoreService ExamScoreService)
        {
            _examScoreService = ExamScoreService;
        }

        [HttpPost("generate-score")]
        public async Task<IActionResult> GenerateScore([FromBody] GenerateScoreRequest request, CancellationToken cancellationToken)
        {
            var report = await _examScoreService.GenerateScoreAsync(request.Question, request.Answer, cancellationToken);
            return Ok(report);
        }

        [HttpPost("get-score-facts")]
        public async Task<IActionResult> GenerateScoreFromFacts([FromBody] FactsRequest request, CancellationToken cancellationToken)
        {
            var response = await _examScoreService.GenerateScoreFromFactsAsync(request.RightFacts, request.Facts, cancellationToken);
            return Ok(response);
        }

        [HttpPost("get-facts")]
        public async Task<IActionResult> GenerateFactsAsync([FromBody] GenerateFactsRequest request, CancellationToken cancellationToken)
        {
            var response = await _examScoreService.GenerateFactsAsync(request.Statement, request.NumberOfFacts, cancellationToken);
            return Ok(response);
        }

        [HttpPost("get-right-answer")]
        public async Task<IActionResult> GenerateRightAnswerAsync([FromBody] GenerateRightAnswerRequest request, CancellationToken cancellationToken)
        {
            var response = await _examScoreService.GenerateRightAnswerAsync(request.Context, request.Question, cancellationToken);
            return Ok(response);
        }        
    }

    public class GenerateRightAnswerRequest
    {
        public string Context { get; set; }
        public string Question { get; set; }
    }

    public class GenerateScoreRequest
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class GenerateFactsRequest
    {
        public string Statement { get; set; }

        public int NumberOfFacts { get; set; }
    }

    public class FactsRequest
    {
        public string RightFacts { get; set; }
        public string Facts { get; set; }
    }
}
