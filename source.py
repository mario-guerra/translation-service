import os

def read_file_content(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        return file.read()

def write_to_markdown(file_path, content):
    with open(file_path, 'w', encoding='utf-8') as file:
        file.write(content)

def iterate_directory(root_dir):
    markdown_content = ""
    for subdir, _, files in os.walk(root_dir):
        for file in files:
            file_path = os.path.join(subdir, file)
            if file_path.endswith('.cs'):
                file_content = read_file_content(file_path)
                relative_path = os.path.relpath(file_path, root_dir)
                markdown_content += f"\\generated\\{relative_path}:\n```\n{file_content}\n```\n\n"
    return markdown_content

def main():
    root_dir = os.getcwd()
    markdown_content = iterate_directory(root_dir)
    output_file_path = os.path.join(root_dir, 'source.md')
    write_to_markdown(output_file_path, markdown_content)

if __name__ == "__main__":
    main()