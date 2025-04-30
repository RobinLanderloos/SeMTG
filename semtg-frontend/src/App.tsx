import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import SearchPage from "./pages/search"; // Adjust this import to your correct path

function App() {
  return (
    <Router>
      <Routes>
        {/* Define the routes here */}
        <Route path="/search" element={<SearchPage />} />
      </Routes>
    </Router>
  );
}

export default App;
