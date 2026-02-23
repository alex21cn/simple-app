# Simple App — Azure SQL Records UI

This React app demonstrates a simple table UI that loads records from an API and supports insert, update and delete operations. It uses Microsoft Identity Platform (MSAL) to sign in users and acquire access tokens for backend calls.

Setup

1. Install dependencies:

```bash
npm install
```

2. Update MSAL configuration in `src/msalConfig.js`:
- Replace `clientId` with your app registration's client id.
- Replace `authority` with your tenant or common authority.
- Replace `apiScopes` with the scope your backend API expects (for example `api://<backend-client-id>/access_as_user`).

3. Configure your backend to accept tokens from this app (or disable auth on the backend for initial testing).

4. Run the app:

```bash
npm start
```

Notes

- The frontend expects the backend endpoints at `https://backend.com/api/load`, `/insert`, `/update`, `/delete`.
- Tokens are requested silently and with popup fallback. For production, consider redirect flows and better error handling.
