// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
// <auto-generated />

using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AudioTranslationService.Models.Service.Models;

namespace AudioTranslationService.Models.Service
{

    public interface IRoutesOperations
    {
        ///<summary>
        /// Submits a payment using the specified payment method and returns success or
        /// yment failure error.
        ///</summary>
        Task<PaymentResponse> ProcessPaymentAsync(PaymentMethod method, Payment payment, string? callbackUrl);
        ///<summary>
        /// Uploads audio content using multipart/form-data, returning a success
        /// onse or an invalid file error.
        ///</summary>
        Task<SuccessResponse> UploadAudioAsync(string contentType, AudioUpload audioupload);
        ///<summary>
        /// Downloads a file (artifact) by providing a container and upload ID,
        /// rning the file content as bytes.
        ///</summary>
        Task<byte[]> DownloadArtifactAsync(string containerName, string uploadId);

    }
}
