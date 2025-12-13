import React, { useState, useEffect } from 'react';
import axios from 'axios';

const API_URL = 'https://localhost:7139/api';

const AddExerciseModal = ({ sessionId, onClose, onExerciseAdded }) => {
    const [availableExercises, setAvailableExercises] = useState([]);
    const [selectedExerciseId, setSelectedExerciseId] = useState('');
    const [orderPosition, setOrderPosition] = useState(1); // Uproszczone ustawienie pozycji

    // R - READ: Pobieranie listy dostêpnych æwiczeñ z bazy (GET /api/Exercises)
    useEffect(() => {
        const fetchExercises = async () => {
            try {
                const response = await axios.get(`${API_URL}/Exercises`);
                setAvailableExercises(response.data);
                if (response.data.length > 0) {
                    setSelectedExerciseId(response.data[0].id);
                }
            } catch (error) {
                console.error("B³¹d podczas pobierania dostêpnych æwiczeñ:", error);
            }
        };
        fetchExercises();
    }, []);

    // C - CREATE: Dodawanie æwiczenia do sesji (POST /api/SessionExercises)
    const handleAddExercise = async () => {
        if (!selectedExerciseId) {
            alert('Proszê wybraæ æwiczenie.');
            return;
        }

        const newSessionExerciseData = {
            sessionId: sessionId,
            exerciseId: parseInt(selectedExerciseId),
            orderPosition: orderPosition // Mo¿na to ustawiæ dynamicznie
        };

        try {
            await axios.post(`${API_URL}/SessionExercises`, newSessionExerciseData);
            
            alert('Æwiczenie dodane do sesji!');
            onExerciseAdded(); // Odœwie¿a listê w ActiveSession
            onClose();         // Zamyka modal
            
        } catch (error) {
            console.error("B³¹d podczas dodawania æwiczenia do sesji:", error.response?.data);
            alert('B³¹d dodawania. SprawdŸ, czy sesja jest aktywna i czy ID jest poprawne.');
        }
    };

    return (
        <div className="modal-backdrop">
            <div className="modal-content">
                <h3>Dodaj Nowe Æwiczenie do Sesji</h3>
                
                <select 
                    value={selectedExerciseId} 
                    onChange={(e) => setSelectedExerciseId(e.target.value)}
                >
                    <option value="">-- Wybierz Æwiczenie --</option>                    {availableExercises.map(ex => (
                        <option key={ex.id} value={ex.id}>
                            {ex.name} ({ex.categoryName})
                        </option>
                    ))}
                </select>
                
                {/* Uproszczone pole OrderPosition dla demonstracji */}
                <input
                    type="number"
                    placeholder="Kolejnoœæ (Order)"
                    value={orderPosition}
                    onChange={(e) => setOrderPosition(parseInt(e.target.value) || 1)}
                />

                <button onClick={handleAddExercise} className="btn-primary">Dodaj</button>
                <button onClick={onClose} className="btn-secondary">Anuluj</button>
            </div>
        </div>
    );
};

export default AddExerciseModal;