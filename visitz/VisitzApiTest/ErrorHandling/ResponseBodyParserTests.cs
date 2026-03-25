using VisitzApi.ErrorHandling;

namespace VisitzApiTest.ErrorHandling;

public class ResponseBodyParserTests
{
    const string success = "Success";

    const string error = "Error";

    const string firstErrorText = "First error text";

    const string secondErrorText = "Second error text";

    const string messageSingle =
        $@"{{
            ""message"": ""{firstErrorText}""
        }}";

    const string errorMessagesArray =
        $@"{{
	        ""message"": [
		        ""{firstErrorText}"",
		        ""{secondErrorText}""
	        ],
	        ""error"": ""BAD REQUEST"",
	        ""statusCode"": 400
        }}";

    const string bodyError =
        $@"{{
	        ""error"": ""{firstErrorText}"",
	        ""statusCode"": 403
        }}";

    const string bodyWithErrorStatus =
        $@"{{
	        ""serverResponse"": {{
		        ""payload"": {{
			        ""status"": ""{error}"",
			        ""errors"": [
				        {{
					        ""error"": ""{firstErrorText}""
				        }}
			        ]
		        }}
	        }}
        }}";

    const string bodyWithImplicitSuccessStatus =
        $@"{{
	        ""serverResponse"": {{
		        ""payload"": {{ }}
	        }}
        }}";

    const string bodyWithExplicitSuccessStatus =
        $@"{{
	        ""serverResponse"": {{
		        ""payload"": {{
			        ""status"": ""{success}""
		        }}
	        }}
        }}";

    const string bodyWithErrorDetail =
        $@"{{
	        ""responseFormAttachment"": {{
		        ""payLoad"": {{
			        ""status"": ""{error}"",
			        ""error"": [{{
					    ""errors"": {{
						    ""errorDetail"": ""{secondErrorText}""
					    }}
				    }}]
		        }}
	        }}
        }}";

    [Theory]
    [InlineData(messageSingle)]
    [InlineData(errorMessagesArray)]
    public void FoundMessageElement(string responseBody)
    {
        ResponseBodyParser bodyParser = new(responseBody);

        Assert.NotNull(bodyParser.FindFirstMessage());
    }

    [Fact]
    public void FoundSingleMessage()
    {
        ResponseBodyParser bodyParser = new(messageSingle);

        Assert.Equal(firstErrorText, bodyParser.GetFirstMessages().FirstOrDefault());
    }

    [Fact]
    public void FoundAllMessages()
    {
        ResponseBodyParser bodyParser = new(errorMessagesArray);

        Assert.Equal(firstErrorText, bodyParser.GetFirstMessages().ElementAt(0));
        Assert.Equal(secondErrorText, bodyParser.GetFirstMessages().ElementAt(1));
    }

    [Fact]
    public void FoundBodyErrorText()
    {
        ResponseBodyParser bodyParser = new(bodyError);

        Assert.Equal(firstErrorText, bodyParser.FindFirstError());
    }

    [Fact]
    public void FoundNestedBodyErrorText()
    {
        ResponseBodyParser bodyParser = new(bodyWithErrorStatus);

        Assert.Equal(firstErrorText, bodyParser.FindFirstError());
    }

    [Fact]
    public void FoundNestedBodyStatus()
    {
        ResponseBodyParser bodyParser = new(bodyWithErrorStatus);
        bool? result = bodyParser.GetSuccessStatusFromBody();

        Assert.NotNull(result);
    }

    [Fact]
    public void NestedBodyStatusIsImplicitSuccess()
    {
        ResponseBodyParser bodyParser = new(bodyWithImplicitSuccessStatus);
        bool? result = bodyParser.GetSuccessStatusFromBody();

        Assert.Null(result);
    }

    [Fact]
    public void NestedBodyStatusIsExplicitSuccess()
    {
        ResponseBodyParser bodyParser = new(bodyWithExplicitSuccessStatus);
        bool? result = bodyParser.GetSuccessStatusFromBody();

        Assert.NotNull(result);
        Assert.True(result);
    }

    [Fact]
    public void FoundNestedErrorDetail()
    {
        ResponseBodyParser bodyParser = new(bodyWithErrorDetail);

        Assert.Equal(secondErrorText, bodyParser.FindFirstError());
    }

    [Fact]
    public void ParseExceptionOnEmptyBody()
    {
        ResponseBodyParser bodyParser = new(string.Empty);

        Assert.NotNull(bodyParser.ParseException);
    }

    [Fact]
    public void ExceptionOnAccessAfterDispose()
    {
        ResponseBodyParser bodyParser = new(messageSingle);
        bodyParser.Dispose();

        Assert.Throws<ObjectDisposedException>(() => bodyParser.FindFirstMessage());
    }
}
