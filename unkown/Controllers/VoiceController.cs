using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OpenAI_API;
using OpenAI_API.Completions;
using Twilio.AspNet.Common;
using Twilio.AspNet.Core;
using Twilio.Http;
using Twilio.TwiML;
using Twilio.TwiML.Voice;

namespace unkown.Controllers
{
    [Route("[controller]")]
    public class VoiceController : TwilioController
    {
        private readonly OpenAIAPI _openAiClient;
        public VoiceController()
        {
            string openAiApiKey = ""; // Replace with your OpenAI API key
            _openAiClient = new OpenAIAPI(openAiApiKey);
        }

  

        [HttpPost("Welcome")]
        public IActionResult Welcome()
        {
            var response = new VoiceResponse();

            // Main menu with options

            var gather = new Gather(
            action: new Uri("/voice/HandleMenuSelection", UriKind.Relative),
            numDigits: 1,
            timeout: 5, // Wait for 5 seconds for input
            actionOnEmptyResult: true // Redirect if no input is received
            );

            gather.Append(new Say("Welcome to the Sarasota Surgical support system. Press 1 for insurance questions, Press 2 for inventory questions, Press 3 for equipment usage questions."));
  
            response.Append(gather);

            // If no input is received, repeat the menu
            response.Say("Sorry, I didn’t receive any input. Redirecting you to the main menu.");
            response.Redirect(new Uri("/voice/Welcome", UriKind.Relative));

            return TwiML(response);
        }

        [HttpPost("HandleMenuSelection")]
        public IActionResult HandleMenuSelection([FromForm] VoiceRequest request)
        {
            var response = new VoiceResponse();
            if (string.IsNullOrEmpty(request.Digits))
            {
                // If no input was provided, return to the main menu
                response.Say("Sorry, you did not provide any input. Redirecting you to the main menu.");
                response.Redirect(new Uri("/voice/Welcome", UriKind.Relative));
                return TwiML(response);
            }
            var selectedOption = request.Digits;

            
            switch (selectedOption)
            {
                case "1":
                    response.Say("You selected insurance questions.");
                    response.Redirect(new Uri("/voice/AskQuestion", UriKind.Relative));
                    break;
                case "2":
                    response.Say("You selected inventory questions.");
                    response.Redirect(new Uri("/voice/AskQuestion", UriKind.Relative));
                    break;
                case "3":
                    response.Say("You selected equipment usage questions.");
                    response.Redirect(new Uri("/voice/AskQuestion", UriKind.Relative));
                    break;
                default:
                    response.Say("Invalid option. Returning to the main menu.");
                    response.Redirect(new Uri("/voice/Welcome", UriKind.Relative));
                    break;
            }

            return TwiML(response);
        }



        [HttpPost("AskQuestion")]
        public IActionResult AskQuestion()
        {
            var response = new VoiceResponse();

            // Inform the user to ask their question and provide a pause for them to start speaking
            response.Say("Please ask your question after the beep.");
            response.Pause(length: 1); // Adds a 1-second pause

            // Gather user input with extended timeout
            var gather = new Gather(action: new Uri("/voice/ProcessQuestion", UriKind.Relative), input: new List<Gather.InputEnum> { Gather.InputEnum.Speech, Gather.InputEnum.Dtmf }, timeout: 5, speechTimeout: "auto");
            gather.FinishOnKey = "#";
            gather.Timeout= 5;
            gather.SpeechTimeout = "auto";
            gather.BargeIn = true;
            response.Append(gather);

            return TwiML(response);
        }

        [HttpPost("ProcessQuestion")]
        public async Task<IActionResult> ProcessQuestion([FromForm] string SpeechResult)
        {
            var response = new VoiceResponse();

            // Step 1: Get the user's spoken question
            if (string.IsNullOrEmpty(SpeechResult))
            {
                response.Say("Sorry, I didn't hear your question. Please ask again.");
                response.Redirect(new Uri("/voice/AskQuestion", UriKind.Relative)); // Redirect to ask again
                return TwiML(response);
            }

            // Step 2: Send the question to GPT agent
            var completionRequest = new CompletionRequest
            {
                Prompt = SpeechResult,
                MaxTokens = 100, // Limit the response length to a short and concise answer
                Temperature = 0.7 // You can adjust this to control randomness. Lower values give more focused responses.
            };

            var gptResponse = await _openAiClient.Completions.CreateCompletionAsync(completionRequest);
            var answer = gptResponse.Completions.FirstOrDefault()?.Text.Trim() ?? "Sorry, I couldn't understand that.";

            // Step 3: Build TwiML to respond with GPT's answer using Gather to allow interruption
            var gather = new Gather(action: new Uri("/voice/HandleMoreQuestions", UriKind.Relative))
            {
                Input = new List<Twilio.TwiML.Voice.Gather.InputEnum>
        {
            Twilio.TwiML.Voice.Gather.InputEnum.Dtmf,
            Twilio.TwiML.Voice.Gather.InputEnum.Speech
        },
                Timeout = 5, // Timeout waiting for input
                SpeechTimeout = "auto", // Auto-detect when the user stops speaking
                BargeIn = true, // Allow interruption
                FinishOnKey = "#", // Optionally finish when '#' is pressed
                Language = "en-US"
            };

            gather.Append(new Say($"Here is the answer to your question: {answer}. Do you have any other questions? Please say Yes or No."));

            response.Append(gather);

            // Step 4: Return the TwiML response
            return TwiML(response);
        }

        [HttpPost("HandleMoreQuestions")]
        public IActionResult HandleMoreQuestions([FromForm] VoiceRequest request)
        {
            var response = new VoiceResponse();
            var userInput = request.SpeechResult ?? request.Digits;

            if (!string.IsNullOrEmpty(userInput))
            {
                // Normalize the input to handle both speech and DTMF
                userInput = userInput.ToLower().Trim();

                // Add checks for common variations of 'yes' and 'no'
                if (userInput == "yes" || userInput == "1" || userInput.Contains("yeah") || userInput.Contains("yep"))
                {
                    response.Append(new Say("Please go ahead and ask your question."));
                    response.Redirect(new Uri("/voice/AskQuestion", UriKind.Relative)); // Redirect to ask a new question
                }
                else if (userInput == "no" || userInput == "2" || userInput.Contains("nope"))
                {
                    response.Append(new Say("Thank you for calling. Have a great day!"));
                    response.Hangup();
                }
                else
                {
                    // If unrecognized input, ask again
                    response.Append(new Say("Sorry, I didn't understand that. Please say Yes or No."));
                    response.Pause(4);
                    response.Redirect(new Uri("/voice/HandleMoreQuestions", UriKind.Relative));
                }
            }
            else
            {
                // If no input at all, repeat the question
                response.Append(new Say("I didn't hear anything. Please say Yes or No."));
                response.Redirect(new Uri("/voice/HandleMoreQuestions", UriKind.Relative));
            }

            return TwiML(response);
        }
    }
}
