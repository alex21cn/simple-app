import axios from 'axios';
import BASE from './env';

const withAuth = (token) => ({
  headers: {
    Authorization: `Bearer ${token}`,
    'Content-Type': 'application/json'
  }
});

export async function loadRecords(token, pageIndex = 0, pageSize = 10) {
  const url = `${BASE}/load?pageIndex=${encodeURIComponent(pageIndex)}&pageSize=${encodeURIComponent(pageSize)}`;
  const res = await axios.get(url, withAuth(token));
  return res.data;
}

export async function insertRecord(token, record) {
  const res = await axios.post(`${BASE}/insert`, record, withAuth(token));
  return res.data;
}

export async function updateRecord(token, record) {
  const res = await axios.put(`${BASE}/update`, record, withAuth(token));
  return res.data;
}

export async function deleteRecord(token, id) {
  const res = await axios.delete(`${BASE}/delete`, {
    ...withAuth(token),
    data: { id }
  });
  return res.data;
}
