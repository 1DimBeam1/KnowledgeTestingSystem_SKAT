namespace InterpretatorService.DTOs
{
    public class CreateAlgorithmRequestDto
    {
        public string AlgoName { get; set; }
        public string CodeContent { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string AllStepsJson { get; set; }
    }
}
