// <auto-generated/>

#nullable disable

using System;
using System.ClientModel;
using System.ClientModel.Primitives;
using System.Threading;
using System.Threading.Tasks;
using TranslationService.Models;

namespace TranslationService
{
    /// <summary></summary>
    public partial class Routes
    {
        private readonly Uri _endpoint;

        /// <summary> Initializes a new instance of Routes for mocking. </summary>
        protected Routes()
        {
        }

        internal Routes(ClientPipeline pipeline, Uri endpoint)
        {
            _endpoint = endpoint;
            Pipeline = pipeline;
        }

        /// <summary> The HTTP pipeline for sending and receiving REST requests and responses. </summary>
        public ClientPipeline Pipeline { get; }

        /// <summary>
        /// [Protocol Method] Submits a payment using the specified payment method and returns success or a payment failure error.
        /// <list type="bullet">
        /// <item>
        /// <description> This <see href="https://aka.ms/azsdk/net/protocol-methods">protocol method</see> allows explicit creation of the request and processing of the response for advanced scenarios. </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="content"> The content to send as the body of the request. </param>
        /// <param name="method"> The method used for paying, e.g., PayPal, Stripe, or CreditCard. </param>
        /// <param name="callbackUrl"> Optional callback URL for receiving payment status updates from the gateway. </param>
        /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        /// <returns> The response returned from the service. </returns>
        public virtual ClientResult ProcessPayment(BinaryContent content, string @method, string callbackUrl = null, RequestOptions options = null)
        {
            Argument.AssertNotNull(content, nameof(content));

            using PipelineMessage message = CreateProcessPaymentRequest(content, @method, callbackUrl, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }

        /// <summary>
        /// [Protocol Method] Submits a payment using the specified payment method and returns success or a payment failure error.
        /// <list type="bullet">
        /// <item>
        /// <description> This <see href="https://aka.ms/azsdk/net/protocol-methods">protocol method</see> allows explicit creation of the request and processing of the response for advanced scenarios. </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="content"> The content to send as the body of the request. </param>
        /// <param name="method"> The method used for paying, e.g., PayPal, Stripe, or CreditCard. </param>
        /// <param name="callbackUrl"> Optional callback URL for receiving payment status updates from the gateway. </param>
        /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        /// <returns> The response returned from the service. </returns>
        public virtual async Task<ClientResult> ProcessPaymentAsync(BinaryContent content, string @method, string callbackUrl = null, RequestOptions options = null)
        {
            Argument.AssertNotNull(content, nameof(content));

            using PipelineMessage message = CreateProcessPaymentRequest(content, @method, callbackUrl, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }

        /// <summary> Submits a payment using the specified payment method and returns success or a payment failure error. </summary>
        /// <param name="payment"> Represents the base payment details gathered from the user. </param>
        /// <param name="method"> The method used for paying, e.g., PayPal, Stripe, or CreditCard. </param>
        /// <param name="callbackUrl"> Optional callback URL for receiving payment status updates from the gateway. </param>
        /// <param name="cancellationToken"> The cancellation token that can be used to cancel the operation. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="payment"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        public virtual ClientResult<PaymentResponse> ProcessPayment(Payment payment, PaymentMethod @method, string callbackUrl = null, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNull(payment, nameof(payment));

            ClientResult result = ProcessPayment(payment, @method.ToSerialString(), callbackUrl, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
            return ClientResult.FromValue((PaymentResponse)result, result.GetRawResponse());
        }

        /// <summary> Submits a payment using the specified payment method and returns success or a payment failure error. </summary>
        /// <param name="payment"> Represents the base payment details gathered from the user. </param>
        /// <param name="method"> The method used for paying, e.g., PayPal, Stripe, or CreditCard. </param>
        /// <param name="callbackUrl"> Optional callback URL for receiving payment status updates from the gateway. </param>
        /// <param name="cancellationToken"> The cancellation token that can be used to cancel the operation. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="payment"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        public virtual async Task<ClientResult<PaymentResponse>> ProcessPaymentAsync(Payment payment, PaymentMethod @method, string callbackUrl = null, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNull(payment, nameof(payment));

            ClientResult result = await ProcessPaymentAsync(payment, @method.ToSerialString(), callbackUrl, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
            return ClientResult.FromValue((PaymentResponse)result, result.GetRawResponse());
        }

        /// <summary>
        /// [Protocol Method] Uploads audio content using multipart/form-data, returning a success response or an invalid file error.
        /// <list type="bullet">
        /// <item>
        /// <description> This <see href="https://aka.ms/azsdk/net/protocol-methods">protocol method</see> allows explicit creation of the request and processing of the response for advanced scenarios. </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="content"> The content to send as the body of the request. </param>
        /// <param name="contentType"> The contentType to use which has the multipart/form-data boundary. </param>
        /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        /// <returns> The response returned from the service. </returns>
        public virtual ClientResult UploadAudio(BinaryContent content, string contentType, RequestOptions options = null)
        {
            Argument.AssertNotNull(content, nameof(content));

            using PipelineMessage message = CreateUploadAudioRequest(content, contentType, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }

        /// <summary>
        /// [Protocol Method] Uploads audio content using multipart/form-data, returning a success response or an invalid file error.
        /// <list type="bullet">
        /// <item>
        /// <description> This <see href="https://aka.ms/azsdk/net/protocol-methods">protocol method</see> allows explicit creation of the request and processing of the response for advanced scenarios. </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="content"> The content to send as the body of the request. </param>
        /// <param name="contentType"> The contentType to use which has the multipart/form-data boundary. </param>
        /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="content"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        /// <returns> The response returned from the service. </returns>
        public virtual async Task<ClientResult> UploadAudioAsync(BinaryContent content, string contentType, RequestOptions options = null)
        {
            Argument.AssertNotNull(content, nameof(content));

            using PipelineMessage message = CreateUploadAudioRequest(content, contentType, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }

        /// <summary>
        /// [Protocol Method] Downloads a file (artifact) by providing a container and upload ID, returning the file content as bytes.
        /// <list type="bullet">
        /// <item>
        /// <description> This <see href="https://aka.ms/azsdk/net/protocol-methods">protocol method</see> allows explicit creation of the request and processing of the response for advanced scenarios. </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="ContainerName"> Identifies the storage container for the artifact. </param>
        /// <param name="uploadId"> Specifies the upload identifier for the target artifact. </param>
        /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="ContainerName"/> or <paramref name="uploadId"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        /// <returns> The response returned from the service. </returns>
        public virtual ClientResult DownloadArtifact(string containerName, string uploadId, RequestOptions options)
        {
            Argument.AssertNotNull(containerName, nameof(containerName));
            Argument.AssertNotNull(uploadId, nameof(uploadId));

            using PipelineMessage message = CreateDownloadArtifactRequest(containerName, uploadId, options);
            return ClientResult.FromResponse(Pipeline.ProcessMessage(message, options));
        }

        /// <summary>
        /// [Protocol Method] Downloads a file (artifact) by providing a container and upload ID, returning the file content as bytes.
        /// <list type="bullet">
        /// <item>
        /// <description> This <see href="https://aka.ms/azsdk/net/protocol-methods">protocol method</see> allows explicit creation of the request and processing of the response for advanced scenarios. </description>
        /// </item>
        /// </list>
        /// </summary>
        /// <param name="ContainerName"> Identifies the storage container for the artifact. </param>
        /// <param name="uploadId"> Specifies the upload identifier for the target artifact. </param>
        /// <param name="options"> The request options, which can override default behaviors of the client pipeline on a per-call basis. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="ContainerName"/> or <paramref name="uploadId"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        /// <returns> The response returned from the service. </returns>
        public virtual async Task<ClientResult> DownloadArtifactAsync(string containerName, string uploadId, RequestOptions options)
        {
            Argument.AssertNotNull(containerName, nameof(containerName));
            Argument.AssertNotNull(uploadId, nameof(uploadId));

            using PipelineMessage message = CreateDownloadArtifactRequest(containerName, uploadId, options);
            return ClientResult.FromResponse(await Pipeline.ProcessMessageAsync(message, options).ConfigureAwait(false));
        }

        /// <summary> Downloads a file (artifact) by providing a container and upload ID, returning the file content as bytes. </summary>
        /// <param name="ContainerName"> Identifies the storage container for the artifact. </param>
        /// <param name="uploadId"> Specifies the upload identifier for the target artifact. </param>
        /// <param name="cancellationToken"> The cancellation token that can be used to cancel the operation. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="ContainerName"/> or <paramref name="uploadId"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        public virtual ClientResult<BinaryData> DownloadArtifact(string containerName, string uploadId, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNull(containerName, nameof(containerName));
            Argument.AssertNotNull(uploadId, nameof(uploadId));

            ClientResult result = DownloadArtifact(containerName, uploadId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null);
            return ClientResult.FromValue(result.GetRawResponse().Content, result.GetRawResponse());
        }

        /// <summary> Downloads a file (artifact) by providing a container and upload ID, returning the file content as bytes. </summary>
        /// <param name="ContainerName"> Identifies the storage container for the artifact. </param>
        /// <param name="uploadId"> Specifies the upload identifier for the target artifact. </param>
        /// <param name="cancellationToken"> The cancellation token that can be used to cancel the operation. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="ContainerName"/> or <paramref name="uploadId"/> is null. </exception>
        /// <exception cref="ClientResultException"> Service returned a non-success status code. </exception>
        public virtual async Task<ClientResult<BinaryData>> DownloadArtifactAsync(string containerName, string uploadId, CancellationToken cancellationToken = default)
        {
            Argument.AssertNotNull(containerName, nameof(containerName));
            Argument.AssertNotNull(uploadId, nameof(uploadId));

            ClientResult result = await DownloadArtifactAsync(containerName, uploadId, cancellationToken.CanBeCanceled ? new RequestOptions { CancellationToken = cancellationToken } : null).ConfigureAwait(false);
            return ClientResult.FromValue(result.GetRawResponse().Content, result.GetRawResponse());
        }
    }
}
