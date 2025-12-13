import React, { useState } from 'react';
import ActiveSession from './components/sessions/ActiveSession';
import './App.css'; 

function App() {
  // Symulujemy, ¿e u¿ytkownik ID 1 rozpocz¹³ sesjê o ID 1
  const [activeSessionId, setActiveSessionId] = useState(1);
  const userId = 1;

  const handleStartSession = () => {
    // W prawdziwej aplikacji to POST /api/TrainingSessions
    setActiveSessionId(2); 
  };
  
  const handleSessionEnd = () => {
      alert(`Sesja ${activeSessionId} zakoñczona pomyœlnie!`);
      setActiveSessionId(null);
  };
  
  // Wymaganie wstêpne do testowania:
  // Upewnij siê, ¿e w bazie masz:
  // 1. U¿ytkownika o ID 1.
  // 2. Aktywn¹ sesjê treningow¹ o ID 1 (stworzon¹ przez POST /api/TrainingSessions).
  // 3. Dodane æwiczenie do sesji o ID 1 (SessionExercise ID).
  
  return (
    <div className="app-container">
      <h1>Training Tracker - Demo Frontend</h1>
      
      {activeSessionId ? (
        <ActiveSession 
          sessionId={activeSessionId} 
          userId={userId}
          onSessionEnd={handleSessionEnd}
        />
      ) : (
        <div>
            <h2>WELCOME WARRIOR</h2>
            <p>Brak aktywnego treningu.</p>
            <button onClick={handleStartSession}>ROZPOCZNIJ NOWY TRENING</button>
        </div>
      )}
    </div>
  );
}

export default App;