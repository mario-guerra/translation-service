// Translation.Frontend/src/App.tsx
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import PaymentForm from './components/PaymentForm.tsx';
import UploadForm from './components/UploadForm.tsx';
import './index.css';

const App: React.FC = () => {
  return (
    <Router>
      <Routes>
        <Route path="/" element={<PaymentForm />} />
        <Route path="/upload" element={<UploadForm />} />
      </Routes>
    </Router>
  );
};

export default App;