using Plugin.Maui.OCR;

namespace NumericalApproximationApp.Services
{
    public class OCRService
    {
        private readonly IOcrService _ocrService;

        public OCRService(IOcrService ocrService)
        {
            _ocrService = ocrService;
        }

        public async Task<string> RecognizeFromImageAsync(byte[] imageData)
        {
            await _ocrService.InitAsync();

            var result = await _ocrService.RecognizeTextAsync(imageData);

            if (result.Success)
            {
                return CleanMathExpression(result.AllText);
            }

            return string.Empty;
        }

        public async Task<byte[]> CapturePhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    using var stream = await photo.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Camera error: {ex.Message}");
            }

            return null;
        }

        public async Task<byte[]> PickPhotoAsync()
        {
            try
            {
                var photo = await MediaPicker.Default.PickPhotoAsync();

                if (photo != null)
                {
                    using var stream = await photo.OpenReadAsync();
                    using var memoryStream = new MemoryStream();
                    await stream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Gallery error: {ex.Message}");
            }

            return null;
        }

        private string CleanMathExpression(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw))
                return string.Empty;

            string cleaned = raw.Trim();

            cleaned = cleaned.Replace("X", "x");
            cleaned = cleaned.Replace("×", "*");
            cleaned = cleaned.Replace("÷", "/");
            cleaned = cleaned.Replace("−", "-");
            cleaned = cleaned.Replace("–", "-");
            cleaned = cleaned.Replace("—", "-");
            cleaned = cleaned.Replace(",", ".");
            cleaned = cleaned.Replace("{", "(");
            cleaned = cleaned.Replace("}", ")");
            cleaned = cleaned.Replace("[", "(");
            cleaned = cleaned.Replace("]", ")");
            cleaned = cleaned.Replace("²", "^2");
            cleaned = cleaned.Replace("³", "^3");
            cleaned = cleaned.Replace("⁴", "^4");

            var validChars = new HashSet<char>("0123456789xX.+-*/^() sincotaqrlgep");
            string result = "";
            foreach (char c in cleaned)
            {
                if (validChars.Contains(c) || char.IsLetter(c))
                    result += c;
            }

            return result.Trim();
        }
    }
}