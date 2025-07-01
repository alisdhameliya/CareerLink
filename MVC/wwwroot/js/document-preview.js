// Global function to handle document preview
function previewDocument(filename) {
    console.log('Preview document called with filename:', filename);
    
    // Check if the filename is valid
    if (!filename) {
        console.error('Filename is empty');
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Document filename is not available'
        });
        return;
    }
    
    // Construct the preview URL using the endpoint
    var previewUrl = `/Admin/preview-document/${encodeURIComponent(filename)}`;
    console.log('Preview URL:', previewUrl);
    
    // Open document in new tab
    var newWindow = window.open(previewUrl, '_blank');
    
    // Check if the window opened successfully
    if (!newWindow) {
        console.error('Failed to open preview window');
        Swal.fire({
            icon: 'error',
            title: 'Error',
            text: 'Failed to open document preview. Please check your browser settings.'
        });
    }
} 