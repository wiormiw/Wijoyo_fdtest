import { useState } from 'react';
import { usePublicBooks } from '../hooks/useBooks';

const PAGE_SIZE = 12;

export default function PublicBookFeed() {
  const [page, setPage] = useState(1);
  const [Search, _] = useState('');
  const [Author, setAuthor] = useState('');
  const [Rating, setRating] = useState<number | ''>('');
  const [SortBy, setSortBy] = useState('dateUploaded');
  const [SortDirection, setSortDirection] = useState('desc');

  const { data, isLoading, isError } = usePublicBooks({
    Author,
    SortBy,
    SortDirection,
    PageNumber: page,
    PageSize: PAGE_SIZE,
    Search,
    Rating: Rating === '' ? undefined : Rating,
  });

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-2 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-3xl font-bold mb-6 text-center">Public Book Feed</h1>
        <form
          className="flex flex-col sm:flex-row flex-wrap gap-4 mb-8 justify-center"
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
          <select
            className="px-4 py-2 border rounded-md w-full sm:w-32 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            value={Rating}
            onChange={e => setRating(e.target.value === '' ? '' : Number(e.target.value))}
          >
            <option value="">All Ratings</option>
            {[1,2,3,4,5].map(r => <option key={r} value={r}>{r}</option>)}
          </select>
          <select
            className="px-4 py-2 border rounded-md w-full sm:w-40 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            value={SortBy}
            onChange={e => setSortBy(e.target.value)}
          >
            <option value="title">Title</option>
            <option value="author">Author</option>
            <option value="rating">Rating</option>
            <option value="dateUploaded">Date Uploaded</option>
          </select>
          <select
            className="px-4 py-2 border rounded-md w-full sm:w-32 focus:outline-none focus:ring-2 focus:ring-indigo-500"
            value={SortDirection}
            onChange={e => setSortDirection(e.target.value)}
          >
            <option value="asc">Ascending</option>
            <option value="desc">Descending</option>
          </select>
          <button
            type="submit"
            className="px-6 py-2 bg-indigo-600 text-white rounded-md hover:bg-indigo-700 transition"
          >
            Search
          </button>
        </form>
        {isLoading ? (
          <div className="flex justify-center items-center min-h-[200px] text-gray-400">Loading...</div>
        ) : isError ? (
          <div className="flex justify-center items-center min-h-[200px] text-red-500">Failed to load books.</div>
        ) : data && data.items.length === 0 ? (
          <div className="flex justify-center items-center min-h-[200px] text-gray-400">No books found.</div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-6">
            {data?.items.map(book => (
              <div key={book.id} className="bg-white rounded-lg shadow hover:shadow-lg transition flex flex-col overflow-hidden">
                {book.coverUrl ? (
                  <img src={book.coverUrl} alt={book.title} className="h-48 w-full object-cover" />
                ) : (
                  <div className="h-48 w-full bg-gray-200 flex items-center justify-center text-gray-400">No Cover</div>
                )}
                <div className="p-4 flex-1 flex flex-col">
                  <h2 className="text-lg font-semibold mb-1 line-clamp-2">{book.title}</h2>
                  <p className="text-sm text-gray-600 mb-2">by {book.author}</p>
                  <div className="flex items-center gap-2 mt-auto">
                    <span className="text-yellow-500 font-bold">{book.rating ?? '-'}</span>
                    <span className="text-xs text-gray-400 ml-auto">{new Date(book.dateUploaded).toLocaleDateString()}</span>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
        {/* Pagination */}
        {data && (data.totalPages > 1) && (
          <div className="flex justify-center items-center gap-2 mt-8">
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
      </div>
    </div>
  );
} 