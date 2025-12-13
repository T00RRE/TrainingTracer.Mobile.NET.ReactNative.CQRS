import React, { useState, useEffect } from 'react';
import axios from 'axios';
import AddExerciseModal from './AddExerciseModel';


const API_URL = 'https://localhost:7139/api';

const SessionExerciseTracker = ({ sessionExercise, onSetAdded }) => {
    const [sets, setSets] = useState([]);
    const [weight, setWeight] = useState(0);
    const [reps, setReps] = useState(0);

    useEffect(() => {
        const fetchSets = async () => {
            try {
                const response = await axios.get(`${API_URL}/ExerciseSets/sessionExercise/${sessionExercise.id}`);
                setSets(response.data);
            } catch (error) {
                console.error("B³¹d podczas pobierania serii:", error);
            }
        };
        fetchSets();
    }, [sessionExercise.id, onSetAdded]); 

    const handleAddSet = async () => {
        if (weight <= 0 || reps <= 0) {
            alert('Waga i powtórzenia musz¹ byæ wiêksze od zera.');
            return;
        }

        const newSetNumber = sets.length + 1;
        const newSetData = {
            sessionExerciseId: sessionExercise.id,
            setNumber: newSetNumber,
            weight: parseFloat(weight),
            reps: parseInt(reps),
        };

        try {
            const newSetId = await axios.post(`${API_URL}/ExerciseSets`, newSetData);
            
            setWeight(0);
            setReps(0);
            onSetAdded(); 

        } catch (error) {
            console.error("B³¹d podczas dodawania serii:", error.response?.data);
            alert('B³¹d dodawania serii. SprawdŸ walidacjê.');
        }
    };

    return (
        <div className="session-exercise-card">
            <h3>{sessionExercise.exerciseName}</h3>
            
            <div className="sets-list">
                <h4>Serie:</h4>
                {sets.map((set, index) => (
                    <p key={set.id}>
                        **Seria {index + 1}:** {set.weight} {sessionExercise.weightUnit} x {set.reps} powt.
                    </p>
                ))}
            </div>

            <div className="add-set-form">
                <input
                    type="number"
                    placeholder="Ciê¿ar (kg/lbs)"
                    value={weight}
                    onChange={(e) => setWeight(e.target.value)}
                />
                <input
                    type="number"
                    placeholder="Powtórzenia"
                    value={reps}
                    onChange={(e) => setReps(e.target.value)}
                />
                <button onClick={handleAddSet}>Dodaj Seriê</button>
            </div>
        </div>
    );
};

// G³ówny widok Aktywnej Sesji
const ActiveSession = ({ sessionId, userId, onSessionEnd }) => {
    const [sessionExercises, setSessionExercises] = useState([]);
    const [isSessionActive, setIsSessionActive] = useState(true);
    const [showAddModal, setShowAddModal] = useState(false); // NOWY STAN
    const [refreshSets, setRefreshSets] = useState(0);

    // R - READ: Pobieranie æwiczeñ w aktywnej sesji
    const fetchSessionData = async () => {
        try {
            const exercisesResponse = await axios.get(`${API_URL}/SessionExercises/${sessionId}`);
            setSessionExercises(exercisesResponse.data);
        } catch (error) {
            console.error("B³¹d podczas pobierania æwiczeñ w sesji:", error);
            // Mo¿emy za³o¿yæ, ¿e sesja nie istnieje i j¹ zakoñczyæ
            setIsSessionActive(false); 
        }
    };

    useEffect(() => {
        if (sessionId) {
            fetchSessionData();
        }
    }, [sessionId, refreshSets]); 

    // U - UPDATE: Zakoñczenie treningu (strona 8)
    const handleEndSession = async () => {
        try {
            // EndSession: true ustawia CompletedAt w backendzie
            const updateData = { notes: "Zakonczono z poziomu aplikacji demo.", endSession: true };
            await axios.put(`${API_URL}/TrainingSessions/${sessionId}`, updateData);
            
            setIsSessionActive(false);
            onSessionEnd(); // Powrót do historii
        } catch (error) {
            console.error("B³¹d podczas koñczenia sesji:", error);
            alert('Nie uda³o siê zakoñczyæ sesji.');
        }
    };

    // Obs³uga odœwie¿ania po dodaniu serii (u¿ywana tak¿e przez AddExerciseModal)
    const handleSetAdded = () => {
        setRefreshSets(prev => prev + 1);
    };

    if (!sessionId || !isSessionActive) {
        return <div>Sesja zosta³a zakoñczona lub nie jest aktywna.</div>;
    }

    return (
        <div className="active-session-container">
            <h2>TRENING: Sesja {sessionId}</h2>
            
            {/* Lista æwiczeñ w sesji (strona 8) */}
            <div className="session-exercises-list">
                {sessionExercises.map(ex => (
                    <SessionExerciseTracker 
                        key={ex.id} 
                        sessionExercise={ex} 
                        onSetAdded={handleSetAdded}
                    />
                ))}
            </div>

            {/* Przycisk DODAJ NOWE ÆWICZENIE - otwiera modal */}
            <button className="add-exercise-btn" onClick={() => setShowAddModal(true)}>
                + DODAJ NOWE ÆWICZENIE
            </button>

            {/* ZAKOÑCZ TRENING (Page 8) */}
            <button className="end-session-btn" onClick={handleEndSession}>
                ZAKOÑCZ TRENING
            </button>

            {/* Renderowanie modalu do dodawania æwiczeñ */}
            {showAddModal && (
                <AddExerciseModal
                    sessionId={sessionId}
                    onClose={() => setShowAddModal(false)}
                    onExerciseAdded={fetchSessionData} // Odœwie¿a listê æwiczeñ w sesji
                />
            )}
        </div>
    );
};

export default ActiveSession;