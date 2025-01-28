import AudioTranslationServiceClient from '../clients/typescript/_bundles/my-lib.js';
// Import the bundled module.  The alias AudioTranslationServiceClient will store the value exported as default
//import AudioTranslationServiceClient from '../clients/typescript/dist/bundle/audio-translation-service-client.js';

// Function to display messages on the UI
function displayMessage(containerId, message, isError = false) {
    const messageArea = document.getElementById(containerId);
    if (messageArea) {
       messageArea.textContent = message;
        if (isError) {
            messageArea.style.color = 'red'; // Make error messages red
        } else {
           messageArea.style.color = 'black'; // Reset to default color for messages
        }
    } else {
      console.error(`Could not find container: ${containerId}`);
    }
}

// Payment Form Handling
const paymentForm = document.getElementById('payment-form');
paymentForm.addEventListener('submit', async function(event) {
    event.preventDefault(); // Prevent the default form submission

    const userEmail = document.getElementById('userEmail').value;
    const amount = parseFloat(document.getElementById('amount').value);
    const service = document.getElementById('service').value;
    const synthesizedAudio = document.getElementById('synthesizedAudio').value === 'true';

    try {
        const client = new AudioTranslationServiceClient();
        const paymentResponse = await client.processPayment({
            userEmail: userEmail,
            amount: amount,
            service: service,
             synthesizedAudio: synthesizedAudio
        });

        displayMessage('payment-message-area', `Payment was successful: ${paymentResponse.status}`);

        // Switch to upload screen after payment
        document.getElementById('payment-screen').classList.remove('active');
        document.getElementById('upload-screen').classList.add('active');

    } catch (error) {
          console.error("Payment Error:", error);
         displayMessage('payment-error', `Payment failed: ${error.message}`, true);
    }
});

// Audio Upload Form Handling
const uploadForm = document.getElementById('upload-form');
uploadForm.addEventListener('submit', async function(event) {
    event.preventDefault(); // Prevent the default form submission

     const audioFile = document.getElementById('audioFile').files[0];
    const langIn = document.getElementById('LangIn').value;
    const langOut = document.getElementById('LangOut').value;

    if (!audioFile) {
       displayMessage('upload-error', 'Please select a valid audio file.', true);
        return;
    }

    if (audioFile.type !== 'audio/wav') {
          displayMessage('upload-error', 'Please use a wav audio file', true);
        return;
    }


    try {
        const client = new AudioTranslationServiceClient();
       const translationResponse = await client.translateAudio({
          audio: audioFile,
           langIn: langIn,
            langOut: langOut
       });

        displayMessage('upload-message-area', `Translation completed: ${translationResponse.transcript}`);

    } catch (error) {
         console.error("Upload Error:", error);
          displayMessage('upload-error', `Audio translation failed: ${error.message}`, true);
    }
});