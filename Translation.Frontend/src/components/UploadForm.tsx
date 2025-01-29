// Translation.Frontend/src/components/UploadForm.tsx
import React, { useState } from 'react';
import apiClient from '../services/apiClient.ts';
import { useLocation } from 'react-router-dom';
import ErrorDisplay from './ErrorDisplay.tsx';

const UploadForm: React.FC = () => {
  const [langIn, setLangIn] = useState('');
  const [langOut, setLangOut] = useState('');
  const [file, setFile] = useState<File | null>(null);
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const [successMessage, setSuccessMessage] = useState('');
  const location = useLocation();
  const userId = location.state?.userId;

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0]);
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccessMessage('');
    setLoading(true);

    if (!file) {
      setError("Please select a file to upload.");
      setLoading(false);
      return;
    }

    const makeUploadRequest = async (retryCount = 0) => {
      try {
        const formData = new FormData();
        formData.append("file", file);
        formData.append("userId", userId);
        formData.append("LangIn", langIn);
        formData.append("LangOut", langOut);

        const response = await apiClient.path("/upload").post({
          body: formData,
          contentType: "multipart/form-data",
        });

        if (response.status === "200") {
          setSuccessMessage("File uploaded successfully! You will receive an email with a download link.");
        } else {
          setError(response.body.message);
        }
      } catch (err: any) {
        console.error("Upload error:", err);
        if (retryCount < 3 && err.message.includes("Failed to fetch")) {
          console.log(`Retrying upload request, attempt ${retryCount + 1}`);
          await new Promise(resolve => setTimeout(resolve, 1000 * (retryCount + 1)));
          return makeUploadRequest(retryCount + 1);
        } else {
          setError("An error occurred during file upload. Please try again later.");
        }
      } finally {
        setLoading(false);
      }
    };

    await makeUploadRequest();
  };

  return (
    <div className="max-w-md mx-auto p-4">
      <h2 className="text-2xl font-bold mb-4">Audio Upload</h2>
      <form onSubmit={handleSubmit} className="space-y-4">
        <ErrorDisplay message={error} />
        {successMessage && <div className="bg-green-100 border border-green-400 text-green-700 px-4 py-3 rounded relative" role="alert">
          <span className="block sm:inline">{successMessage}</span>
        </div>}
        <div>
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="langIn">Input Language</label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="langIn"
            type="text"
            placeholder="Enter input language"
            value={langIn}
            onChange={(e) => setLangIn(e.target.value)}
            required
          />
        </div>
        <div>
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="langOut">Output Language</label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="langOut"
            type="text"
            placeholder="Enter output language"
            value={langOut}
            onChange={(e) => setLangOut(e.target.value)}
            required
          />
        </div>
        <div>
          <label className="block text-gray-700 text-sm font-bold mb-2" htmlFor="file">Audio File</label>
          <input
            className="shadow appearance-none border rounded w-full py-2 px-3 text-gray-700 leading-tight focus:outline-none focus:shadow-outline"
            id="file"
            type="file"
            onChange={handleFileChange}
            required
          />
        </div>
        <button
          type="submit"
          className={`bg-blue-500 hover:bg-blue-700 text-white font-bold py-2 px-4 rounded focus:outline-none focus:shadow-outline ${loading ? 'opacity-50 cursor-not-allowed' : ''}`}
          disabled={loading}
        >
          {loading ? 'Uploading...' : 'Upload'}
        </button>
      </form>
    </div>
  );
};

export default UploadForm;