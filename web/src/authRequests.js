import { apiScopes } from './msalConfig';

export const loginRequest = {
  scopes: ['openid', 'profile', 'User.Read']
};

export const tokenRequest = {
  scopes: apiScopes
};
