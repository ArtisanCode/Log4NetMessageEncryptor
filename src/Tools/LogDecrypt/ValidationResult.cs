namespace LogDecrypt
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }

        public ValidationResult()
        {
            IsValid = true;
        }
    }
}