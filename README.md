Project Title: ElderCare Connect - Voice AI Healthcare Assistance
Overview
For the ElderCareConnect, users can call 855-785-3838 to interact with the AI voice system, which provides health-related assistance, reminders, and answers to general or specific healthcare questions. This dedicated phone line ensures easy access for elderly users, particularly those with limited digital literacy, offering a seamless experience for managing daily health tasks and getting medical information over the phone.

By incorporating this phone number, the app makes health support more accessible to users who may not be comfortable using mobile or digital applications.

This system provides culturally sensitive, multilingual responses on topics such as chronic disease management, medication adherence, and general health tips, helping users with limited digital literacy or visual impairments.

Key Features
Voice-Activated Healthcare Assistance:

Users call a dedicated number and ask health-related questions.
AI responds with answers based on reputable medical sources.
Culturally Relevant Multilingual Responses:

The platform tailors responses to the cultural and linguistic needs of minority communities.
Chronic Disease Management Education:

Provides educational content to help users manage chronic conditions such as diabetes and hypertension.
Medication Support:

Offers voice-activated instructions for medication management, including dosage information and reminders.
Community Health Integration:

Collaborates with local healthcare providers and organizations to offer information relevant to underserved communities.
Problem Statement
Elderly individuals and those who are blind or visually impaired in minority communities face several challenges in accessing healthcare services:

Limited access to healthcare professionals.
Difficulty understanding medical instructions due to low health literacy.
Digital exclusion, as many existing telehealth services require smartphones or internet access.
ElderCare Connect overcomes these barriers by providing voice-based telehealth assistance that is accessible via a regular phone call, ensuring users can access healthcare guidance without needing digital devices or the internet.

Technologies Used
ASP.NET Core: Used for building the backend of the voice interaction service.
Twilio: Provides the telephony services for managing phone calls and capturing user input via speech or keypad input.
OpenAI GPT: Integrated to provide intelligent and dynamic responses to healthcare-related questions based on natural language input.
Installation and Setup
To set up this project, follow these steps:

Clone the Repository:

bash
Copy code
git clone https://github.com/GodCodeWeed/ElderCareConnect.git
cd ElderCareConnect
Set Up Twilio:

Sign up for Twilio and get your Account SID, Auth Token, and a phone number.
Update Twilio settings in the code (if applicable).
Set Up OpenAI:

Sign up for OpenAI API.
Get your API key and replace string openAiApiKey = ""; in VoiceController.cs with your actual key.
Run the Application:

Open the project in your IDE (such as Visual Studio).
Build and run the application:
bash
Copy code
dotnet build
dotnet run
Configure Webhooks:

Use Twilio’s console to set the appropriate webhooks for handling incoming calls and routing them to the /voice/Welcome endpoint.
Usage
Welcome Interaction: When users call, they are greeted with a menu of options, including insurance questions, inventory, and equipment usage.
Question Handling: After selecting an option, users are prompted to ask their questions. The AI system (powered by OpenAI) will analyze the input and provide a response.
Follow-up: Users can ask follow-up questions or end the call based on their needs.
API Endpoints
POST /voice/Welcome: Main entry point for the call, providing users with a menu of options.
POST /voice/HandleMenuSelection: Handles the user's selection from the main menu.
POST /voice/AskQuestion: Prompts users to ask their questions.
POST /voice/ProcessQuestion: Sends the user's spoken question to OpenAI and processes the response.
POST /voice/HandleMoreQuestions: Asks the user if they have more questions and continues interaction.
Future Enhancements
Health Monitoring Device Integration: Connect to health monitoring devices for real-time health updates.
Emergency Services: Direct integration with medical emergency services for critical situations.
Contribution
Contributions are welcome! To contribute:

Fork the repository.
Create a new branch (git checkout -b feature-name).
Commit your changes (git commit -am 'Add feature').
Push to the branch (git push origin feature-name).
Create a Pull Request.
License
This project is licensed under the MIT License - see the LICENSE file for details.
