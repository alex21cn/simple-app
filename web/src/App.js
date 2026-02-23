import React from 'react';
import { useMsal, useIsAuthenticated } from '@azure/msal-react';
import { loginRequest } from './authRequests';
import RecordsTable from './components/RecordsTable';

function App() {
  const { instance } = useMsal();
  const isAuthenticated = useIsAuthenticated();

  const handleLogin = async () => {
    try {
      await instance.loginPopup(loginRequest);
    } catch (e) {
      console.error(e);
    }
  };

  const handleLogout = () => {
    instance.logoutPopup();
  };

  return (
    <div style={{ padding: 20, fontFamily: 'Arial, sans-serif' }}>
      <h1>Records Manager</h1>
      <div style={{ marginBottom: 16 }}>
        {isAuthenticated ? (
          <>
            <button onClick={handleLogout}>Logout</button>
          </>
        ) : (
          <button onClick={handleLogin}>Login with Microsoft</button>
        )}
      </div>

      {isAuthenticated ? (
        <RecordsTable />
      ) : (
        <p>Please sign-in to view and manage records.</p>
      )}
    </div>
  );
}

export default App;
