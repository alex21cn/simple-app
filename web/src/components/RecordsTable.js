import React, { useEffect, useState } from 'react';
import { useMsal, useAccount } from '@azure/msal-react';
import { tokenRequest } from '../authRequests';
import { loadRecords, insertRecord, updateRecord, deleteRecord } from '../api';

function RecordsTable() {
  const { instance, accounts } = useMsal();
  const account = useAccount(accounts[0] || {});
  const [token, setToken] = useState(null);
  const [rows, setRows] = useState([]);
  const [loading, setLoading] = useState(false);
  const [form, setForm] = useState({ id: '', filename: '', type: '', path: '' });
  const [editingId, setEditingId] = useState(null);
  const [totalCount, setTotalCount] = useState(0);
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const totalPages = pageSize > 0 ? Math.max(1, Math.ceil(totalCount / pageSize)) : 1;

  useEffect(() => {
    const getToken = async () => {
      try {
        const resp = await instance.acquireTokenSilent({ ...tokenRequest, account });
        setToken(resp.accessToken);
      } catch (e) {
        try {
          const resp = await instance.acquireTokenPopup({ ...tokenRequest, account });
          setToken(resp.accessToken);
        } catch (err) {
          console.error(err);
        }
      }
    };

    if (account) getToken();
  }, [instance, account]);

  const fetchPage = async (pIndex, pSize) => {
    if (!token) return;
    setLoading(true);
    try {
      const data = await loadRecords(token, pIndex, pSize);
      if (!data) return;
      setRows(Array.isArray(data.items) ? data.items : []);
      setTotalCount(typeof data.totalCount === 'number' ? data.totalCount : 0);
      setPageIndex(typeof data.pageIndex === 'number' ? data.pageIndex : pIndex);
      setPageSize(typeof data.pageSize === 'number' ? data.pageSize : pSize);
    } catch (e) {
      console.error(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!token) return;
    fetchPage(pageIndex, pageSize);
  }, [token, pageIndex, pageSize]);

  const refresh = async () => {
    await fetchPage(pageIndex, pageSize);
  };

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value });

  const handleAdd = async () => {
    if (!token) return;
    await insertRecord(token, { filename: form.filename, type: form.type, path: form.path });
    setForm({ id: '', filename: '', type: '', path: '' });
    refresh();
  };

  const startEdit = (r) => {
    setEditingId(r.id);
    setForm({ id: r.id, filename: r.filename || '', type: r.type || '', path: r.path || '' });
  };

  const handleUpdate = async () => {
    if (!token) return;
    await updateRecord(token, { id: form.id, filename: form.filename, type: form.type, path: form.path });
    setEditingId(null);
    setForm({ id: '', filename: '', type: '', path: '' });
    refresh();
  };

  const handleDelete = async (id) => {
    if (!token) return;
    if (!window.confirm('Delete this record?')) return;
    await deleteRecord(token, id);
    refresh();
  };

  const goToPage = (idx) => {
    const clamped = Math.max(0, Math.min(idx, totalPages - 1));
    setPageIndex(clamped);
  };

  const handlePageSizeChange = (e) => {
    const newSize = parseInt(e.target.value, 10) || 10;
    setPageSize(newSize);
    setPageIndex(0);
  };

  return (
    <div>
      <h2>Records</h2>
      <div style={{ marginBottom: 12 }}>
        <input name="filename" placeholder="Filename" value={form.filename} onChange={handleChange} />
        <input name="type" placeholder="Type" value={form.type} onChange={handleChange} />
        <input name="path" placeholder="Path" value={form.path} onChange={handleChange} />
        {editingId ? (
          <>
            <button onClick={handleUpdate}>Save</button>
            <button onClick={() => { setEditingId(null); setForm({ id: '', filename: '', type: '', path: '' }); }}>Cancel</button>
          </>
        ) : (
          <button onClick={handleAdd}>Add</button>
        )}
        <button onClick={refresh} style={{ marginLeft: 8 }}>Refresh</button>
      </div>

      {loading ? (
        <p>Loading...</p>
      ) : (
        <>
        <table border="1" cellPadding="6" style={{ borderCollapse: 'collapse' }}>
          <thead>
            <tr>
              <th>ID</th>
              <th>Filename</th>
              <th>Type</th>
              <th>Path</th>
              <th>Posted</th>
              <th>Last Updated</th>
              <th>Linked Nodes</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {rows.map((r) => (
              <tr key={r.id}>
                <td>{r.id}</td>
                <td>{r.filename}</td>
                <td>{r.type}</td>
                <td>{r.path}</td>
                <td>{r.postedDate ? new Date(r.postedDate).toLocaleString() : ''}</td>
                <td>{r.lastUpdatedDate ? new Date(r.lastUpdatedDate).toLocaleString() : ''}</td>
                <td>{r.numberOfLinkedNodes}</td>
                <td>
                  <button onClick={() => startEdit(r)}>Edit</button>
                  <button onClick={() => handleDelete(r.id)} style={{ marginLeft: 8 }}>Delete</button>
                </td>
              </tr>
            ))}
            {rows.length === 0 && (
              <tr>
                <td colSpan={8}>No records</td>
              </tr>
            )}
          </tbody>
        </table>
        <div style={{ marginTop: 8, display: 'flex', alignItems: 'center', gap: 12 }}>
          <div>
            <a
              href="#"
              onClick={(e) => { e.preventDefault(); goToPage(0); }}
              aria-disabled={pageIndex <= 0}
              style={{ color: pageIndex <= 0 ? '#999' : '#06c' }}
            >
              First
            </a>
            <a
              href="#"
              onClick={(e) => { e.preventDefault(); goToPage(pageIndex - 1); }}
              aria-disabled={pageIndex <= 0}
              style={{ marginLeft: 12, color: pageIndex <= 0 ? '#999' : '#06c' }}
            >
              Prev
            </a>
            <label style={{ marginLeft: 12 }}>
              Page size
              <select value={pageSize} onChange={handlePageSizeChange} style={{ marginLeft: 8 }}>
                <option value={10}>10</option>
                <option value={20}>20</option>
                <option value={50}>50</option>
                <option value={100}>100</option>
              </select>
            </label>
            <a
              href="#"
              onClick={(e) => { e.preventDefault(); goToPage(pageIndex + 1); }}
              aria-disabled={(pageIndex + 1) >= totalPages}
              style={{ marginLeft: 12, color: (pageIndex + 1) >= totalPages ? '#999' : '#06c' }}
            >
              Next
            </a>
            <a
              href="#"
              onClick={(e) => { e.preventDefault(); goToPage(totalPages - 1); }}
              aria-disabled={(pageIndex + 1) >= totalPages}
              style={{ marginLeft: 12, color: (pageIndex + 1) >= totalPages ? '#999' : '#06c' }}
            >
              Last
            </a>
          </div>
          <div>
            <span>Page {pageIndex + 1} of {totalPages}</span>
            <span style={{ marginLeft: 12 }}>Showing {rows.length} of {totalCount}</span>
          </div>
        </div>
        </>
      )}
    </div>
  );
}

export default RecordsTable;
