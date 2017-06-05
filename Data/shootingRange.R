library(tidyverse)

#load data file
trajectoryData = read_csv("trajectoryData.txt", col_names = FALSE)


ggplot(data = trajectoryData, mapping = aes(x = X5, y = X4))+
  geom_point(mapping = aes(color = as.character(X1)))
