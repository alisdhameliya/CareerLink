// Sample skills data
const skills = [
    // Frontend
    "React", "Angular", "Vue.js", "TypeScript", "JavaScript", "HTML5", "CSS3", "Sass", "Less",
    // Backend
    "Node.js", "Python", "Java", ".NET", "PHP", "Ruby", "Go", "Rust",
    // Databases
    "SQL", "MongoDB", "PostgreSQL", "MySQL", "Redis", "Oracle", "SQLite",
    // Cloud & DevOps
    "AWS", "Azure", "GCP", "Docker", "Kubernetes", "Jenkins", "Terraform", "Ansible",
    // Other
    "Git", "Agile", "CI/CD", "REST APIs", "GraphQL", "Microservices", "System Design"
];

// Sample job listings data
const jobListings = [
    
];

// Initialize all components when document is ready
$(document).ready(function() {
    initializeSkillsAutocomplete();
    initializeDropdowns();
    initializeJobListings();
});

// Initialize skills autocomplete
function initializeSkillsAutocomplete() {
    $("#skillsAutocomplete").kendoAutoComplete({
        dataSource: skills,
        placeholder: "Type to search skills...",
        filter: "contains",
        minLength: 1,
        select: function(e) {
            var selectedSkill = e.dataItem;
            if (!selectedSkill) return;

            // Check if skill is already selected
            if ($("#selectedSkills").find(`[data-skill="${selectedSkill}"]`).length === 0) {
                // Add selected skill as a tag
                var tag = $(`
                    <span class="badge bg-primary d-flex align-items-center gap-2" data-skill="${selectedSkill}">
                        ${selectedSkill}
                        <i class="fas fa-times cursor-pointer" style="cursor: pointer;"></i>
                    </span>
                `);

                // Add remove functionality
                tag.find('.fa-times').click(function() {
                    tag.remove();
                });

                $("#selectedSkills").append(tag);
            }

            // Clear the input
            $("#skillsAutocomplete").data("kendoAutoComplete").value("");
        }
    });
}

// Initialize all dropdowns
function initializeDropdowns() {
    $("#experienceLevel").kendoDropDownList({
        dataSource: ["Entry Level", "Mid Level", "Senior Level", "Lead"],
        value: "Entry Level"
    });

    $("#salaryRange").kendoDropDownList({
        dataSource: ["Less than ₹2L", "₹2L - ₹3L", "₹3L - ₹5L", "₹5L - ₹8L", "₹8L - ₹12L", "₹12L+"],
        value: "₹3L - ₹5L"
    });

    $("#datePosted").kendoDropDownList({
        dataSource: ["Last 24 hours", "Last 7 days", "Last 30 days", "Any time"],
        value: "Last 7 days"
    });

    $("#sortOptions").kendoDropDownList({
        dataSource: ["Most Recent", "Salary: High to Low", "Salary: Low to High"],
        value: "Most Recent"
    });
}

// Initialize job listings ListView
function initializeJobListings() {
    $("#jobListings").kendoListView({
        dataSource: {
            data: jobListings
        },
        template: `
            <div class="job-card">
                <div class="d-flex align-items-center mb-3">
                    <img src="https://via.placeholder.com/48" alt="Company" class="company-logo me-3">
                    <div class="flex-grow-1">
                        <h5 class="mb-1">#: title #</h5>
                        <p class="text-muted mb-0">#: company #</p>
                    </div>
                    <button class="k-button k-button-md k-rounded-md k-button-solid k-button-solid-base k-button-icon">
                        <i class="far fa-bookmark"></i>
                    </button>
                </div>
                <div class="d-flex gap-4 mb-3 text-muted">
                    <span><i class="fas fa-map-marker-alt me-1"></i>#: location #</span>
                    <span><i class="fas fa-dollar-sign me-1"></i>#: salary #</span>
                    <span><i class="fas fa-clock me-1"></i>#: type #</span>
                </div>
                <p class="mb-3">#: description #</p>
                <div class="d-flex flex-wrap gap-2 mb-3">
                    # for(var i = 0; i < skills.length; i++) { #
                        <span>#: skills[i] #</span>
                    # } #
                </div>
                <div class="d-flex justify-content-between align-items-center">
                    <small class="text-muted">Posted #: posted #</small>
                    <button class="k-button k-button-md k-rounded-md k-button-solid k-button-solid-success">
                        Apply Now
                    </button>
                </div>
            </div>
        `
    });
} 