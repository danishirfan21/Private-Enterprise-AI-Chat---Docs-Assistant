# Private Enterprise AI Chat + Document Assistant

A full-stack, 100% local AI chat application with document analysis capabilities. Built with .NET 8, React + TypeScript, and Ollama for complete data privacy.

## Features

- **Private & Local**: 100% local execution using Ollama - no cloud APIs, complete data privacy
- **AI Chat**: Streaming chat responses with multiple model support (Llama 3, Mistral, Phi-3, etc.)
- **Document Analysis**: Upload and process PDF, DOCX, and TXT files
- **RAG (Retrieval Augmented Generation)**: Semantic search with source citations
- **Multi-Model Support**: Switch between different Ollama models
- **Conversation Management**: Create, view, and delete chat conversations
- **Dark/Light Theme**: Modern UI with theme toggle
- **Responsive Design**: Works on desktop and mobile devices

## Architecture

```
┌─────────────┐      ┌──────────────┐      ┌─────────────┐
│   React     │──────│  .NET 8 API  │──────│   Ollama    │
│  Frontend   │ HTTP │   Backend    │ HTTP │  (Local AI) │
│  (Port 80)  │      │  (Port 5000) │      │ (Port 11434)│
└─────────────┘      └──────────────┘      └─────────────┘
                              │
                     ┌────────┴────────┐
                     │  In-Memory      │
                     │  Vector Store   │
                     └─────────────────┘
```

### Tech Stack

**Backend (.NET 8)**
- ASP.NET Core Web API
- Microsoft.Extensions.AI (Ollama integration)
- Serilog (logging)
- FluentValidation
- PdfPig (PDF extraction)
- DocumentFormat.OpenXml (DOCX extraction)

**Frontend (React + TypeScript)**
- Vite
- Zustand (state management)
- TanStack Query (server state)
- TailwindCSS (styling)
- React Markdown (markdown rendering)
- Lucide React (icons)

**AI & Infrastructure**
- Ollama (local LLM hosting)
- Docker Compose (containerization)

## Prerequisites

### Option 1: Docker (Recommended)
- Docker Desktop 20.10+
- Docker Compose 2.0+
- 8GB+ RAM (16GB recommended for larger models)

### Option 2: Local Development
- .NET 8 SDK
- Node.js 20+
- Ollama installed locally
- 8GB+ RAM (16GB recommended)

## Quick Start with Docker

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd "Private Enterprise AI Chat + Docs Assistant"
   ```

2. **Start all services**
   ```bash
   docker-compose up -d
   ```

   This will start:
   - Ollama (port 11434)
   - Backend API (port 5000)
   - Frontend (port 80)

3. **Pull Ollama models**
   ```bash
   docker exec -it enterprise-ai-ollama ollama pull llama3
   docker exec -it enterprise-ai-ollama ollama pull nomic-embed-text
   ```

   Available models:
   - `llama3` (recommended, ~4.7GB)
   - `mistral` (~4.1GB)
   - `phi3` (~2.3GB)
   - `llama3:70b` (large, ~40GB)

4. **Access the application**
   - Open http://localhost in your browser
   - Start chatting!

## Local Development Setup

### Backend

1. **Navigate to backend directory**
   ```bash
   cd backend
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update configuration** (if needed)
   Edit `src/EnterpriseAI.API/appsettings.json`

4. **Run the API**
   ```bash
   cd src/EnterpriseAI.API
   dotnet run
   ```

   API will be available at http://localhost:5000

### Frontend

1. **Navigate to frontend directory**
   ```bash
   cd frontend
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Create environment file**
   ```bash
   # Create .env file
   echo "VITE_API_BASE_URL=http://localhost:5000" > .env
   ```

4. **Run development server**
   ```bash
   npm run dev
   ```

   Frontend will be available at http://localhost:5173

### Ollama Setup

1. **Install Ollama**
   - Download from https://ollama.ai
   - Follow installation instructions for your OS

2. **Pull models**
   ```bash
   ollama pull llama3
   ollama pull nomic-embed-text
   ```

3. **Verify Ollama is running**
   ```bash
   curl http://localhost:11434/api/version
   ```

## API Documentation

### Endpoints

**Health Check**
- `GET /health` - Health status

**Models**
- `GET /api/models` - List available Ollama models

**Documents**
- `POST /api/documents` - Upload document (multipart/form-data)
- `GET /api/documents` - List uploaded documents
- `DELETE /api/documents/{id}` - Delete document

**Chat**
- `POST /api/chat/stream` - Chat with SSE streaming
  ```json
  {
    "conversationId": "optional-guid",
    "message": "Your question here",
    "modelName": "llama3:latest",
    "temperature": 0.7,
    "maxTokens": 2000,
    "useRAG": true,
    "systemPrompt": "Optional system prompt"
  }
  ```

**Conversations**
- `GET /api/conversations` - List conversations
- `GET /api/conversations/{id}` - Get conversation with messages
- `POST /api/conversations` - Create new conversation
- `DELETE /api/conversations/{id}` - Delete conversation

## Configuration

### Backend (appsettings.json)

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "DefaultChatModel": "llama3:latest",
    "DefaultEmbeddingModel": "nomic-embed-text"
  },
  "DocumentProcessing": {
    "MaxFileSizeMB": 10,
    "ChunkSize": 512,
    "ChunkOverlap": 50
  },
  "VectorStore": {
    "SimilarityThreshold": 0.7,
    "TopKResults": 5
  }
}
```

### Frontend (.env)

```env
VITE_API_BASE_URL=http://localhost:5000
```

## How It Works

### RAG Pipeline

1. **Document Upload**
   - User uploads PDF/DOCX/TXT file
   - Backend extracts text content
   - Text is chunked into 512-token segments with 50-token overlap
   - Each chunk is embedded using Ollama's `nomic-embed-text`
   - Embeddings stored in in-memory vector store

2. **Chat Query with RAG**
   - User sends a message
   - Message is embedded using the same model
   - Cosine similarity search finds top 5 relevant chunks (threshold > 0.7)
   - Relevant chunks are injected into the prompt as context
   - LLM generates response using context + user query
   - Response streams back to frontend via SSE
   - Source citations included in response

### Vector Similarity Search

Cosine similarity formula:
```
similarity = (A·B) / (||A|| × ||B||)
```

Where:
- A and B are embedding vectors
- · represents dot product
- ||A|| and ||B|| are vector magnitudes

## Usage

### Uploading Documents

1. Click the Documents panel (right sidebar on desktop)
2. Drag and drop files or click to browse
3. Supported formats: PDF, DOCX, TXT (max 10MB)
4. Wait for processing to complete
5. Documents are now searchable in RAG mode

### Chatting

1. Type your message in the input box at the bottom
2. Press Enter to send (Shift+Enter for new line)
3. Responses stream in real-time
4. If RAG is enabled and documents are uploaded, relevant sources will be cited

### Settings

Click the Settings icon in the header to configure:
- **Model**: Select from available Ollama models
- **Temperature**: 0.0 (deterministic) to 2.0 (creative)
- **Max Tokens**: Maximum response length (100-4000)
- **Use RAG**: Toggle document context on/off
- **System Prompt**: Customize AI behavior

## Troubleshooting

### Ollama Connection Issues

```bash
# Check if Ollama is running
curl http://localhost:11434/api/version

# Check available models
ollama list

# Pull missing models
ollama pull llama3
ollama pull nomic-embed-text
```

### Backend API Issues

```bash
# Check backend health
curl http://localhost:5000/health

# View backend logs (Docker)
docker logs enterprise-ai-backend

# View backend logs (local)
# Check terminal where dotnet run is running
```

### Frontend Issues

```bash
# Check if API is accessible
curl http://localhost:5000/api/models

# Rebuild frontend
cd frontend
npm run build

# Clear browser cache
# Hard refresh (Ctrl+Shift+R or Cmd+Shift+R)
```

### Document Processing Errors

- **File too large**: Max file size is 10MB (configurable in appsettings.json)
- **Unsupported format**: Only PDF, DOCX, TXT are supported
- **Extraction failed**: PDF may be encrypted or corrupted

### Memory Issues

- Large models require more RAM:
  - `llama3:7b` - 8GB RAM
  - `llama3:70b` - 48GB RAM
- Reduce `DocumentProcessing.ChunkSize` if running low on memory
- Limit number of uploaded documents

## Performance Tips

1. **Use appropriate model sizes**
   - Start with `llama3:7b` or `mistral`
   - Only use larger models if you have sufficient RAM

2. **Adjust chunk parameters**
   - Larger chunks = more context but slower processing
   - Smaller chunks = faster but less context

3. **Limit concurrent conversations**
   - In-memory storage is not persistent
   - Restart clears all data

4. **Optimize Docker resources**
   - Allocate sufficient RAM to Docker Desktop
   - Enable multi-core CPU usage

## Development

### Project Structure

```
.
├── backend/
│   ├── src/
│   │   ├── EnterpriseAI.API/          # Web API
│   │   ├── EnterpriseAI.Core/         # Domain models
│   │   └── EnterpriseAI.Infrastructure/ # Implementations
│   └── Dockerfile
├── frontend/
│   ├── src/
│   │   ├── api/          # API client
│   │   ├── components/   # React components
│   │   ├── hooks/        # Custom hooks
│   │   ├── store/        # Zustand stores
│   │   └── types/        # TypeScript types
│   ├── Dockerfile
│   └── nginx.conf
├── docker-compose.yml
└── README.md
```

### Building for Production

**Backend**
```bash
cd backend
dotnet publish -c Release -o ./publish
```

**Frontend**
```bash
cd frontend
npm run build
# Output in dist/
```

**Docker Images**
```bash
# Build all images
docker-compose build

# Build specific service
docker-compose build backend
docker-compose build frontend
```

## Security Considerations

- **100% Local**: No data leaves your machine
- **No Authentication**: Add authentication if exposing publicly
- **CORS**: Configured for localhost, update for production
- **HTTPS**: Use reverse proxy (nginx, Caddy) for HTTPS in production
- **File Validation**: Validates file types and sizes
- **Input Sanitization**: Uses FluentValidation

## Limitations

- **In-Memory Storage**: Data is lost on restart (not persistent)
- **Single User**: Not designed for multi-user environments
- **No Authentication**: No user management or access control
- **Limited File Types**: Only PDF, DOCX, TXT supported
- **Max File Size**: 10MB limit (configurable)
- **No GPU Acceleration**: Ollama uses CPU by default (GPU support available)

## Future Enhancements

- [ ] Persistent storage (PostgreSQL + pgvector)
- [ ] User authentication and authorization
- [ ] Multi-user support
- [ ] More file format support (Excel, PowerPoint, HTML)
- [ ] Conversation export (JSON, Markdown)
- [ ] Advanced RAG features (hybrid search, reranking)
- [ ] GPU acceleration configuration
- [ ] Rate limiting
- [ ] Audit logging

## License

MIT License - see LICENSE file for details

## Contributing

Contributions are welcome! Please open an issue or submit a pull request.

## Support

For issues or questions:
1. Check the Troubleshooting section
2. Review Ollama documentation: https://ollama.ai/docs
3. Open an issue on GitHub

## Acknowledgments

- Ollama team for local LLM hosting
- Microsoft for .NET and Microsoft.Extensions.AI
- React and TailwindCSS communities
- Open source contributors
