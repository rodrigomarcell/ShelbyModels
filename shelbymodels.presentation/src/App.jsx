// src/App.js
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import LoginPage from './components/LoginPage';
import SuccessPage from './components/SuccessPage';
import ErrorPage from './components/ErrorPage';

const App = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<LoginPage />} />
                <Route path="/success" element={<SuccessPage />} />
                <Route path="/error" element={<ErrorPage />} />
            </Routes>
        </Router>
    );
};

export default App;
