const DEV_BASE = 'https://localhost:7129';
const PROD_BASE = 'https://hdbportal-efgrgkg5hzefapb0.canadacentral-01.azurewebsites.net/api';

const BASE = process.env.NODE_ENV === 'production' ? PROD_BASE : DEV_BASE;

export default BASE;
