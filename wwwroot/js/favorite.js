document.addEventListener("DOMContentLoaded", function () {
  const favoriteButtons = document.querySelectorAll(".favorite-btn");

  favoriteButtons.forEach(button => {
      button.addEventListener("click", function () {
          const propertyId = this.dataset.propertyId || 0;
          const demandId = this.dataset.demandId || 0;

          fetch("/Home/ToggleFavorite", {
              method: "POST",
              headers: {
                  "Content-Type": "application/json",
              },
              body: JSON.stringify({ propertyId, demandId }),
          })
              .then(response => response.json())
              .then(data => {
                  if (data.success) {
                      this.classList.toggle("btn-outline-danger");
                      this.classList.toggle("btn-danger");
                  }
              });
      });
  });
});