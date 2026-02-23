export const msalConfig = {
  auth: {
    // TODO: Replace the following placeholders with your app registration values
    clientId: '0cfc38f7-9c1d-482b-a3e3-cd1685706b77',
    authority: 'https://login.microsoftonline.com/cddc1229-ac2a-4b97-b78a-0e5cacb5865c',
    redirectUri: window.location.origin
  },
  cache: {
    cacheLocation: 'localStorage',
    storeAuthStateInCookie: false
  }
};

// Scopes for access token to call the backend API. Replace with your API scope.
export const apiScopes = [
  // Example: 'api://<backend-client-id>/access_as_user'
  'api://0cfc38f7-9c1d-482b-a3e3-cd1685706b77/access_as_user'
];
