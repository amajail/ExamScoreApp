using System.Text.Json;
using ExamScoreApp.Api.Models;
using ExamScoreApp.Core.Application.Services;
using ExamScoreApp.Core.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

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
        public async Task<IActionResult> GenerateScore([FromBody] string rightAnswer,
                                                       string answer,
                                                       CancellationToken cancellationToken)
        {

            var report = await _examScoreService.GenerateScoreAsync(rightAnswer, answer, cancellationToken);
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
