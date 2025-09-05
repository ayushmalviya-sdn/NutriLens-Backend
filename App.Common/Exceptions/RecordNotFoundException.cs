namespace App.Common.Exceptions
{
    public class RecordNotFoundException(string message = "Record not found.") : Exception(message);
}
