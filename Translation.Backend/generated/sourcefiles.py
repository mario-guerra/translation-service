import os

def read_file_content(file_path):
    with open(file_path, 'r', encoding='utf-8') as file:
        return file.read()

def write_to_markdown(file_path, content):
    with open(file_path, 'w', encoding='utf-8') as file:
        file.write(content)

def is_binary(file_path):
    try:
        with open(file_path, 'rb') as file:
            chunk = file.read(1024)
            if b'\0' in chunk:
                return True
    except Exception as e:
        print(f"Error reading file {file_path}: {e}")
        return True
    return False

def iterate_directory(root_dir):
    markdown_content = ""
    for subdir, _, files in os.walk(root_dir):
        for file in files:
            file_path = os.path.join(subdir, file)
            if file_path.endswith('.cs') and file != 'sourcefiles.py' and file != 'source.md' and not is_binary(file_path):
                print(f"Reading file: {file_path}")
                file_content = read_file_content(file_path)
                relative_path = os.path.relpath(file_path, root_dir)
                markdown_content += f"\\{relative_path}:\n```\n{file_content}\n```\n\n"
    return markdown_content

def main():
    root_dir = os.getcwd()
    print(f"Root directory: {root_dir}")
    markdown_content = iterate_directory(root_dir)
    output_file_path = os.path.join(root_dir, 'source.md')
    write_to_markdown(output_file_path, markdown_content)

if __name__ == "__main__":
    main()