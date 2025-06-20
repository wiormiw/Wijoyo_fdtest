import { useEffect, useState } from 'react';
import {
  useUserBooks,
  useCreateBook,
  useUpdateBook,
  useDeleteBook,
  useUploadBookCover,
} from '../hooks/useBooks';
import type { Book } from '../api/books';

const PAGE_SIZE = 10;

function BookFormModal({ open, onClose, onSubmit, initial, uploadCover }: {
  open: boolean;
  onClose: () => void;
  onSubmit: (data: any) => void;
  initial?: Partial<Book>;
  uploadCover: ReturnType<typeof useUploadBookCover>;
}) {
  const [step, setStep] = useState(1);
  const [title, setTitle] = useState('');
  const [author, setAuthor] = useState('');
  const [description, setDescription] = useState('');
  const [rating, setRating] = useState(0);
  const [file, setFile] = useState<File | undefined>();
  const [coverUrl, setCoverUrl] = useState<string>('');
  const [uploading, setUploading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  // Sync props to state when 'initial' changes
  useEffect(() => {
    setTitle(initial?.title || '');
    setAuthor(initial?.author || '');
    setDescription(initial?.description || '');
    setRating(initial?.rating || 0);
    setFile(undefined); // reset file when editing different book
    setCoverUrl(initial?.coverUrl || '');
    setStep(1);
    setError(null);
  }, [initial, open]);

  const handleUpload = async () => {
    if (!file) return;
    setUploading(true);
    setError(null);
    try {
      const url = await uploadCover.mutateAsync(file);
      setCoverUrl(url);
      setStep(2);
    } catch {
      setError('Failed to upload image.');
    } finally {
      setUploading(false);
    }
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    onSubmit({ title, author, description, rating, coverUrl });
  };

  if (!open) return null;

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black bg-opacity-40">
      <div className="bg-white rounded-2xl shadow-2xl p-8 w-full max-w-md relative">
        <button className="absolute top-2 right-2 text-gray-400 hover:text-gray-600 text-2xl" onClick={onClose}>&times;</button>
        <div className="mb-4 flex justify-center gap-2">
          <div className={`w-3 h-3 rounded-full ${step === 1 ? 'bg-indigo-600' : 'bg-gray-300'}`}></div>
          <div className={`w-3 h-3 rounded-full ${step === 2 ? 'bg-indigo-600' : 'bg-gray-300'}`}></div>
        </div>
        {step === 1 ? (
          <div>
            <h2 className="text-xl font-bold mb-4">{initial ? 'Edit Cover' : 'Upload Cover'}</h2>
            {coverUrl ? (
              <div className="mb-4 flex flex-col items-center">
                <img src={coverUrl} alt="Cover Preview" className="h-32 w-24 object-cover rounded shadow mb-2" />
                <button className="text-sm text-indigo-600 underline" onClick={() => { setCoverUrl(''); setFile(undefined); setStep(1); }}>Change Image</button>
              </div>
            ) : (
              <>
                <input type="file" accept="image/*" className="mb-4" onChange={e => setFile(e.target.files?.[0])} />
                {file && (
                  <img src={URL.createObjectURL(file)} alt="Preview" className="h-32 w-24 object-cover rounded shadow mb-2 mx-auto" />
                )}
                {error && <div className="text-red-500 text-sm mb-2">{error}</div>}
                <div className="flex justify-end gap-2">
                  <button type="button" className="px-4 py-2 bg-gray-200 rounded" onClick={onClose}>Cancel</button>
                  <button type="button" disabled={!file || uploading} className="px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700" onClick={handleUpload}>
                    {uploading ? 'Uploading...' : 'Upload & Next'}
                  </button>
                </div>
              </>
            )}
            <div className="flex justify-end gap-2 mt-4">
              <button type="button" className="px-4 py-2 bg-gray-200 rounded" onClick={onClose}>Cancel</button>
              <button
                type="button"
                className={`px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700 ${!coverUrl ? 'opacity-50 cursor-not-allowed' : ''}`}
                onClick={() => {
                  if (!coverUrl) {
                    setError('Please upload a cover image before continuing.');
                  } else {
                    setStep(2);
                  }
                }}
                disabled={!coverUrl}
              >
                Next
              </button>
            </div>
          </div>
        ) : (
          <form className="space-y-4" onSubmit={handleSubmit}>
            <h2 className="text-xl font-bold mb-4">{initial ? 'Edit Book' : 'Add Book'}</h2>
            <div>
              <label className="block text-sm font-medium text-gray-700">Title</label>
              <input className="mt-1 block w-full border rounded px-3 py-2" value={title} onChange={e => setTitle(e.target.value)} required />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700">Author</label>
              <input className="mt-1 block w-full border rounded px-3 py-2" value={author} onChange={e => setAuthor(e.target.value)} required />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700">Description</label>
              <textarea className="mt-1 block w-full border rounded px-3 py-2" value={description} onChange={e => setDescription(e.target.value)} />
            </div>
            <div>
              <label className="block text-sm font-medium text-gray-700">Rating</label>
              <input type="number" min={0} max={5} className="mt-1 block w-full border rounded px-3 py-2" value={rating} onChange={e => setRating(Number(e.target.value))} />
            </div>
            <div className="flex justify-between gap-2">
              <button type="button" className="px-4 py-2 bg-gray-200 rounded" onClick={() => setStep(1)}>Back</button>
              <button
                type="submit"
                className={`px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700 ${!coverUrl ? 'opacity-50 cursor-not-allowed' : ''}`}
                disabled={!coverUrl}
              >
                {initial ? 'Update' : 'Create'}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  );
}

export default function Books() {
  const [page, setPage] = useState(1);
  const [Author, setAuthor] = useState('');
  const [modalOpen, setModalOpen] = useState(false);
  const [editBook, setEditBook] = useState<Book | null>(null);

  const { data, isLoading, isError } = useUserBooks({
    PageNumber: page,
    PageSize: PAGE_SIZE,
    Author: Author.trim() !== '' ? Author : null,
    SortBy: 'dateUploaded',
    SortDirection: 'desc',
  });
  const createBook = useCreateBook();
  const updateBook = useUpdateBook();
  const deleteBook = useDeleteBook();
  const uploadCover = useUploadBookCover();

  const handleCreate = async (form: any) => {
    console.log('form:', form); // Should log the object with all fields
    await createBook.mutateAsync({ ...form });
    setModalOpen(false);
  };
  const handleEdit = async (form: any) => {
    if (!editBook) return;
    try {
      await updateBook.mutateAsync({ id: editBook.id, payload: { ...form, BookId: editBook.id } });
      setEditBook(null);
      setModalOpen(false);
    } catch {}
  };
  const handleDelete = async (id: string) => {
    if (!window.confirm('Delete this book?')) return;
    await deleteBook.mutateAsync(id);
  };

  return (
    <div className="max-w-7xl mx-auto py-8 px-2">
      <div className="flex flex-col sm:flex-row justify-between items-center mb-6 gap-4">
        <h2 className="text-2xl font-bold">My Books</h2>
        <button className="px-4 py-2 bg-indigo-600 text-white rounded-lg hover:bg-indigo-700 transition font-medium shadow" onClick={() => { setEditBook(null); setModalOpen(true); }}>
          + Add Book
        </button>
      </div>
      <form
        className="flex flex-col sm:flex-row gap-4 mb-6 justify-center"
        onSubmit={e => {
          e.preventDefault();
          setPage(1);
        }}
      >
        <input
          type="text"
          placeholder="Filter by author..."
          className="px-4 py-2 border rounded-md w-full sm:w-64 focus:outline-none focus:ring-2 focus:ring-indigo-500"
          value={Author}
          onChange={e => setAuthor(e.target.value)}
        />
        <button
          type="submit"
          className="px-6 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 transition"
        >
          Filter
        </button>
      </form>
      <div className="bg-white shadow-lg rounded-2xl overflow-x-auto">
        <table className="min-w-full divide-y divide-gray-200">
          <thead className="bg-gray-100 rounded-t-2xl">
            <tr>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Cover</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Title</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Author</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Rating</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Uploaded</th>
              <th className="px-4 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Actions</th>
            </tr>
          </thead>
          <tbody className="bg-white divide-y divide-gray-200">
            {isLoading ? (
              <tr>
                <td colSpan={6} className="text-center py-8 text-gray-400">Loading...</td>
              </tr>
            ) : isError ? (
              <tr>
                <td colSpan={6} className="text-center py-8 text-red-500">Failed to load books.</td>
              </tr>
            ) : data && data.items.length === 0 ? (
              <tr>
                <td colSpan={6} className="text-center py-8 text-gray-400">No books found.</td>
              </tr>
            ) : (
              data?.items.map(book => (
                <tr key={book.id} className="hover:bg-gray-50 transition">
                  <td className="px-4 py-3">
                    {book.coverUrl ? (
                      <img src={book.coverUrl} alt={book.title} className="h-16 w-12 object-cover rounded shadow" />
                    ) : (
                      <div className="h-16 w-12 bg-gray-200 rounded flex items-center justify-center text-gray-400">No Cover</div>
                    )}
                  </td>
                  <td className="px-4 py-3 font-semibold text-gray-900">{book.title}</td>
                  <td className="px-4 py-3 text-gray-700">{book.author}</td>
                  <td className="px-4 py-3 text-yellow-500 font-bold">{book.rating ?? '-'}</td>
                  <td className="px-4 py-3 text-gray-500 text-sm">{new Date(book.dateUploaded).toLocaleDateString()}</td>
                  <td className="px-4 py-3 flex gap-2">
                    <button className="px-2 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 text-xs" onClick={() => { setEditBook(book); setModalOpen(true); }}>Edit</button>
                    <button className="px-2 py-1 bg-red-500 text-white rounded hover:bg-red-600 text-xs" onClick={() => handleDelete(book.id)}>Delete</button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
      {/* Pagination */}
      {data && (data.totalPages > 1) && (
        <div className="flex justify-center items-center gap-2 mt-6">
          <button
            className="px-3 py-1 rounded bg-gray-200 hover:bg-gray-300 disabled:opacity-50"
            onClick={() => setPage(p => Math.max(1, p - 1))}
            disabled={page === 1}
          >
            Previous
          </button>
          <span className="text-gray-700 font-medium">
            Page {page} of {data.totalPages}
          </span>
          <button
            className="px-3 py-1 rounded bg-gray-200 hover:bg-gray-300 disabled:opacity-50"
            onClick={() => setPage(p => Math.min(data.totalPages, p + 1))}
            disabled={page === data.totalPages}
          >
            Next
          </button>
        </div>
      )}
      <BookFormModal
        open={modalOpen}
        onClose={() => { setModalOpen(false); setEditBook(null); }}
        onSubmit={editBook ? handleEdit : handleCreate}
        initial={editBook || undefined}
        uploadCover={uploadCover}
      />
    </div>
  );
} 