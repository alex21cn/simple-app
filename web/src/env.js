const DEV_BASE = 'https://localhost:7129';
const PROD_BASE = '/api';

const BASE = process.env.NODE_ENV === 'production' ? PROD_BASE : DEV_BASE;

export default BASE;
